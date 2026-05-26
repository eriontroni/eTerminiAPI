using eTerminiAPI.Application.Interfaces.Caching;
using eTerminiAPI.Application.Interfaces.Notifications;
using eTerminiAPI.Application.Interfaces.Repositories;
using eTerminiAPI.Application.Interfaces.Services;
using eTerminiAPI.Infrastructure.BackgroundServices;
using eTerminiAPI.Infrastructure.Caching;
using eTerminiAPI.Infrastructure.Notifications;
using eTerminiAPI.Infrastructure.Persistence;
using eTerminiAPI.Infrastructure.Repositories;
using eTerminiAPI.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eTerminiAPI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        var redisConnection = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = configuration["Cache:InstanceName"] ?? "eTermini:";
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        services.AddScoped<ICacheService, RedisCacheService>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<ITimeSlotService, TimeSlotService>();

        services.Configure<ReminderOptions>(configuration.GetSection(ReminderOptions.SectionName));
        services.AddScoped<IAppointmentNotifier, LogAppointmentNotifier>();
        services.AddHostedService<AppointmentReminderService>();

        return services;
    }
}
