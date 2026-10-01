using EComerce.Application.DTOs.Chat;
using EComerce.Domain.Common;

namespace EComerce.Application.Interfaces;

/// <summary>
/// Servicio de aplicación para gestión de mensajes de chat.
/// </summary>
public interface IChatService
{
    /// <summary>
    /// Envía un mensaje de un usuario a otro.
    /// </summary>
    Task<Result<ChatMessageDto>> SendMessageAsync(
        string senderId,
        SendMessageRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el historial de conversación entre dos usuarios.
    /// </summary>
    Task<Result<IReadOnlyList<ChatMessageDto>>> GetConversationAsync(
        string user1Id,
        string user2Id,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marca los mensajes recibidos por el usuario como leídos.
    /// </summary>
    Task<Result> MarkAsReadAsync(
        string receiverId,
        string senderId,
        CancellationToken cancellationToken = default);
}
