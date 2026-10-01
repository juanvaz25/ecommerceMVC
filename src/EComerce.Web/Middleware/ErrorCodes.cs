namespace EComerce.Web.Middleware;

/// <summary>
/// Catálogo de códigos de error internos.
/// Permite al cliente identificar errores sin depender de mensajes de texto.
/// </summary>
/// <remarks>
/// Prefijos de categoría:
///   GEN  = Errores generales / no clasificados
///   VAL  = Errores de validación
///   AUTH = Errores de autenticación/autorización
///   DB   = Errores de base de datos
///   CHAT = Errores del módulo de chat
/// </remarks>
public static class ErrorCodes
{
    // ─── Generales ────────────────────────────────────────────────────────────
    /// <summary>Error interno no controlado.</summary>
    public const string InternalServerError = "GEN-001";

    /// <summary>Recurso no encontrado.</summary>
    public const string NotFound = "GEN-002";

    /// <summary>Método o endpoint no soportado.</summary>
    public const string MethodNotAllowed = "GEN-003";

    // ─── Validación ───────────────────────────────────────────────────────────
    /// <summary>Uno o más campos de la petición son inválidos.</summary>
    public const string ValidationFailed = "VAL-001";

    /// <summary>El cuerpo de la petición no tiene el formato esperado.</summary>
    public const string InvalidRequestBody = "VAL-002";

    // ─── Autenticación / Autorización ─────────────────────────────────────────
    /// <summary>El usuario no está autenticado.</summary>
    public const string Unauthorized = "AUTH-001";

    /// <summary>El usuario no tiene permisos para el recurso.</summary>
    public const string Forbidden = "AUTH-002";

    // ─── Base de datos ────────────────────────────────────────────────────────
    /// <summary>Error de concurrencia al guardar datos.</summary>
    public const string ConcurrencyConflict = "DB-001";

    // ─── Chat ─────────────────────────────────────────────────────────────────
    /// <summary>El destinatario del mensaje no existe.</summary>
    public const string ChatReceiverNotFound = "CHAT-001";

    /// <summary>No se puede enviar un mensaje a uno mismo.</summary>
    public const string ChatSelfMessage = "CHAT-002";
}
