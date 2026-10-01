namespace EComerce.Application.Interfaces;

/// <summary>
/// Abstracción del servicio de usuario de Identity.
/// Desacopla Application de la implementación concreta de ASP.NET Identity.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>Id del usuario autenticado actualmente.</summary>
    string? UserId { get; }

    /// <summary>Nombre de usuario del usuario autenticado.</summary>
    string? UserName { get; }

    /// <summary>Indica si hay un usuario autenticado.</summary>
    bool IsAuthenticated { get; }
}
