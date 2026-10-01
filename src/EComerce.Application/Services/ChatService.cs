using EComerce.Application.DTOs.Chat;
using EComerce.Application.Interfaces;
using EComerce.Domain.Common;
using EComerce.Domain.Entities;
using EComerce.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace EComerce.Application.Services;

/// <summary>
/// Implementación del servicio de chat.
/// Orquesta el dominio sin lógica de infraestructura.
/// </summary>
public class ChatService : IChatService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ChatService> _logger;

    public ChatService(IUnitOfWork unitOfWork, ILogger<ChatService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ChatMessageDto>> SendMessageAsync(
        string senderId,
        SendMessageRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Usuario {SenderId} enviando mensaje a {ReceiverId}", senderId, request.ReceiverId);

        var message = ChatMessage.Create(senderId, request.ReceiverId, request.Content);

        await _unitOfWork.ChatMessages.AddAsync(message, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // El nombre del sender se resuelve en la capa Web al emitir el hub.
        var dto = MapToDto(message, senderId);

        return Result.Success(dto);
    }

    public async Task<Result<IReadOnlyList<ChatMessageDto>>> GetConversationAsync(
        string user1Id,
        string user2Id,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var messages = await _unitOfWork.ChatMessages.GetConversationAsync(
            user1Id, user2Id, pageNumber, pageSize, cancellationToken);

        var dtos = messages
            .Select(m => MapToDto(m, m.SenderId))
            .ToList()
            .AsReadOnly();

        return Result.Success<IReadOnlyList<ChatMessageDto>>(dtos);
    }

    public async Task<Result> MarkAsReadAsync(
        string receiverId,
        string senderId,
        CancellationToken cancellationToken = default)
    {
        var messages = await _unitOfWork.ChatMessages.GetConversationAsync(
            senderId, receiverId, cancellationToken: cancellationToken);

        var unread = messages
            .Where(m => m.ReceiverId == receiverId && !m.IsRead)
            .ToList();

        foreach (var msg in unread)
        {
            msg.MarkAsRead();
            _unitOfWork.ChatMessages.Update(msg);
        }

        if (unread.Count > 0)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static ChatMessageDto MapToDto(ChatMessage msg, string senderName) =>
        new(
            msg.Id,
            msg.SenderId,
            senderName,
            msg.ReceiverId,
            msg.Content,
            msg.IsRead,
            msg.ReadAt,
            msg.CreatedAt
        );
}
