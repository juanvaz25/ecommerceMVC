using EComerce.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace EComerce.Web.Services;

/// <summary>
/// Implementación de ICurrentUserService usando IHttpContextAccessor.
/// Resuelve el usuario autenticado del contexto HTTP actual.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId =>
        _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

    public string? UserName =>
        _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}
