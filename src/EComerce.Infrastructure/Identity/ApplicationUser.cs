using Microsoft.AspNetCore.Identity;

namespace EComerce.Infrastructure.Identity;

/// <summary>
/// Extensión del usuario de Identity con campos adicionales del dominio.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public string FullName => $"{FirstName} {LastName}".Trim();
}
