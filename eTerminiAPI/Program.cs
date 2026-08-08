using System.Text;
using eTerminiAPI.API.Hubs;
using eTerminiAPI.Application.Interfaces.Realtime;
using eTerminiAPI.Infrastructure;
using eTerminiAPI.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/etermini.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Prapa Cloudflare Tunnel / reverse proxy: respekto X-Forwarded-Proto & X-Forwarded-For,
// përndryshe ASP.NET Core i sheh kërkesat si http dhe gjeneron redirect/link të gabuar.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // Proxy-t janë brenda rrjetit të Docker-it — nuk dihen paraprakisht IP-të.
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

// Origjinat lejohen nga konfigurimi (Cors:AllowedOrigins), me fallback në portat lokale të Vite.
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>();

if (allowedOrigins is null || allowedOrigins.Length == 0)
{
    allowedOrigins = ["http://localhost:5173", "http://localhost:5174"];
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddHealthChecks();

var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CitizenOrAbove",    p => p.RequireRole("Citizen", "Staff", "InstitutionAdmin", "SuperAdmin"));
    options.AddPolicy("StaffOrAbove",      p => p.RequireRole("Staff", "InstitutionAdmin", "SuperAdmin"));
    options.AddPolicy("InstitutionAdmin",  p => p.RequireRole("InstitutionAdmin", "SuperAdmin"));
    options.AddPolicy("SuperAdmin",        p => p.RequireRole("SuperAdmin"));
});
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddSingleton<ISlotAvailabilityBroadcaster, SignalRSlotBroadcaster>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Vendos JWT token: Bearer {token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Configuration.GetValue("Database:MigrateOnStartup", true))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    if (app.Configuration.GetValue("Database:SeedDemoData", true))
    {
        await DbSeeder.SeedAsync(db);
    }
}

app.UseForwardedHeaders();

if (app.Environment.IsDevelopment() || app.Configuration.GetValue("Swagger:Enabled", false))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Prapa tunelit TLS-i mbyllet te Cloudflare — redirect-i brenda kontejnerit do të krijonte lak.
if (app.Configuration.GetValue("Hosting:UseHttpsRedirection", true))
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireAuthorization();
app.MapHub<AppointmentsHub>("/hubs/appointments");
app.MapHealthChecks("/health").AllowAnonymous();

app.Run();
