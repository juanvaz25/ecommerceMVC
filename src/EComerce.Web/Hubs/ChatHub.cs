using EComerce.Application.DTOs.Chat;
using EComerce.Application.Interfaces;
using EComerce.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace EComerce.Web.Hubs;

/// <summary>
/// Hub de SignalR para chat privado usuario a usuario.
/// Solo accesible para usuarios autenticados.
/// </summary>
[Authorize]
public class ChatHub : Hub
{
    private readonly IChatService _chatService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ChatHub> _logger;

    /// <summary>
    /// Diccionario estático (en memoria) que mapea UserId → ConnectionId.
    /// En producción, reemplazar por Redis o similar para múltiples instancias.
    /// </summary>
    private static readonly Dictionary<string, string> _userConnections = new();

    public ChatHub(
        IChatService chatService,
        UserManager<ApplicationUser> userManager,
        ILogger<ChatHub> logger)
    {
        _chatService = chatService;
        _userManager = userManager;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier!;
        _userConnections[userId] = Context.ConnectionId;

        _logger.LogInformation("Usuario {UserId} conectado al hub de chat. ConnectionId: {ConnectionId}",
            userId, Context.ConnectionId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier!;
        _userConnections.Remove(userId);

        _logger.LogInformation("Usuario {UserId} desconectado del hub de chat.", userId);

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Envía un mensaje privado a otro usuario.
    /// Llamado por el cliente: await connection.invoke("SendPrivateMessage", receiverId, content)
    /// </summary>
    public async Task SendPrivateMessage(string receiverId, string content)
    {
        var senderId = Context.UserIdentifier!;

        if (senderId == receiverId)
        {
            await Clients.Caller.SendAsync("Error", "No puedes enviarte mensajes a ti mismo.");
            return;
        }

        var request = new SendMessageRequest(receiverId, content);
        var result = await _chatService.SendMessageAsync(senderId, request);

        if (result.IsFailure)
        {
            await Clients.Caller.SendAsync("Error", result.Error);
            return;
        }

        var message = result.Value!;

        // Notificar al emisor que el mensaje fue enviado
        await Clients.Caller.SendAsync("MessageSent", message);

        // Notificar al receptor si está conectado
        if (_userConnections.TryGetValue(receiverId, out var receiverConnectionId))
        {
            await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", message);
        }
    }

    /// <summary>
    /// Marca los mensajes de una conversación como leídos.
    /// </summary>
    public async Task MarkMessagesAsRead(string senderId)
    {
        var receiverId = Context.UserIdentifier!;
        await _chatService.MarkAsReadAsync(receiverId, senderId);

        // Notificar al sender que sus mensajes fueron leídos
        if (_userConnections.TryGetValue(senderId, out var senderConnectionId))
        {
            await Clients.Client(senderConnectionId).SendAsync("MessagesRead", receiverId);
        }
    }
}
