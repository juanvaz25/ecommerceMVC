using System.ComponentModel.DataAnnotations;

namespace EComerce.Application.DTOs.Chat;

/// <summary>
/// DTO de entrada para enviar un mensaje de chat.
/// </summary>
public record SendMessageRequest(
    [Required(ErrorMessage = "El destinatario es obligatorio.")]
    string ReceiverId,

    [Required(ErrorMessage = "El contenido no puede estar vacío.")]
    [MaxLength(2000, ErrorMessage = "El mensaje no puede superar 2000 caracteres.")]
    string Content
);
