using eTerminiAPI.Application.Interfaces.Repositories;
using eTerminiAPI.Application.Interfaces.Services;
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

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
