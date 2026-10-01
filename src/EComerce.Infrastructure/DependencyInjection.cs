using EComerce.Domain.Interfaces;
using EComerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EComerce.Infrastructure;

/// <summary>
/// Registro de dependencias de la capa Infrastructure.
/// El registro de Identity se hace en la capa Web por depender de ASP.NET Core.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Entity Framework Core + SQL Server
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
            ));

        // Repositorios y UoW
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
