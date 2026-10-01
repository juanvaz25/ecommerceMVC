namespace EComerce.Application.DTOs.Chat;

/// <summary>
/// DTO de salida para un mensaje de chat.
/// </summary>
public record ChatMessageDto(
    Guid Id,
    string SenderId,
    string SenderName,
    string ReceiverId,
    string Content,
    bool IsRead,
    DateTime? ReadAt,
    DateTime CreatedAt
);
