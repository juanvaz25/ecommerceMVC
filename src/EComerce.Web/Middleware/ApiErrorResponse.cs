namespace EComerce.Web.Middleware;

/// <summary>
/// Formato estándar de respuesta de error de la API.
/// Garantiza consistencia y no expone información sensible.
/// </summary>
public sealed class ApiErrorResponse
{
    /// <summary>Código de estado HTTP.</summary>
    public int Status { get; init; }

    /// <summary>Título corto del error (no variable, apto para i18n).</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Código interno documentado. Permite al cliente mapear el error sin depender del mensaje.
    /// Ver: /docs/error-codes.md
    /// </summary>
    public string ErrorCode { get; init; } = string.Empty;

    /// <summary>Descripción legible del error (solo en Development).</summary>
    public string? Detail { get; init; }

    /// <summary>Errores de validación por campo. Solo presente en errores 422.</summary>
    public IDictionary<string, string[]>? ValidationErrors { get; init; }

    /// <summary>Id de correlación para rastreo en logs.</summary>
    public string TraceId { get; init; } = string.Empty;

    /// <summary>Timestamp UTC del error.</summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
