using EComerce.Application.Interfaces;
using EComerce.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EComerce.Application;

/// <summary>
/// Registro de dependencias de la capa Application.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IChatService, ChatService>();

        return services;
    }
}
