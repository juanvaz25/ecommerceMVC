namespace EComerce.Web.Models;

/// <summary>
/// ViewModel con la información básica de un usuario para mostrar en el chat.
/// </summary>
public class UserSummaryViewModel
{
    public string Id { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }

    /// <summary>Nombre para mostrar: nombre completo o email si no tiene nombre.</summary>
    public string DisplayName =>
        string.IsNullOrWhiteSpace(FirstName + LastName)
            ? Email
            : $"{FirstName} {LastName}".Trim();

    /// <summary>Inicial para el avatar.</summary>
    public string Initial => DisplayName[0].ToString().ToUpper();
}
