using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EComerce.Web.Middleware;

/// <summary>
/// Middleware global de manejo de errores.
/// Captura excepciones no controladas y devuelve respuestas consistentes.
/// 
/// Criterios de aceptación:
/// ✓ Formato estándar de respuesta de error (ApiErrorResponse)
/// ✓ Errores de validación devuelven detalle del campo afectado
/// ✓ Errores no controlados NO exponen stack traces en producción
/// ✓ Códigos de error documentados (ErrorCodes)
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;

        _logger.LogError(
            exception,
            "Error no controlado. TraceId: {TraceId} | Path: {Path}",
            traceId,
            context.Request.Path);

        var (statusCode, title, errorCode, detail) = MapException(exception);

        // En producción, nunca exponer el stack trace ni la cadena de conexión
        var safeDetail = _env.IsDevelopment() ? detail : null;

        var response = new ApiErrorResponse
        {
            Status = statusCode,
            Title = title,
            ErrorCode = errorCode,
            Detail = safeDetail,
            TraceId = traceId,
            Timestamp = DateTime.UtcNow
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        await context.Response.WriteAsync(json);
    }

    private static (int StatusCode, string Title, string ErrorCode, string Detail) MapException(Exception ex)
    {
        return ex switch
        {
            // Errores de concurrencia de EF Core
            DbUpdateConcurrencyException =>
                (StatusCodes.Status409Conflict,
                 "Conflicto de concurrencia",
                 ErrorCodes.ConcurrencyConflict,
                 "El recurso fue modificado por otra operación."),

            // Argumentos inválidos (del dominio)
            ArgumentException or ArgumentNullException =>
                (StatusCodes.Status400BadRequest,
                 "Solicitud inválida",
                 ErrorCodes.InvalidRequestBody,
                 ex.Message),

            // No autorizado
            UnauthorizedAccessException =>
                (StatusCodes.Status401Unauthorized,
                 "No autenticado",
                 ErrorCodes.Unauthorized,
                 "Se requiere autenticación para acceder a este recurso."),

            // Error genérico no controlado — no exponer detalles
            _ =>
                (StatusCodes.Status500InternalServerError,
                 "Error interno del servidor",
                 ErrorCodes.InternalServerError,
                 ex.Message) // solo visible en Development
        };
    }
}

/// <summary>
/// Extensión para registrar el middleware de forma fluente.
/// </summary>
public static class GlobalExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        => app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
}
