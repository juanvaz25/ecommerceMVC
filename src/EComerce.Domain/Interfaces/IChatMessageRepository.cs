using EComerce.Domain.Entities;

namespace EComerce.Domain.Interfaces;

/// <summary>
/// Repositorio especializado para mensajes de chat.
/// </summary>
public interface IChatMessageRepository : IRepository<ChatMessage>
{
    /// <summary>
    /// Obtiene la conversación entre dos usuarios, ordenada por fecha.
    /// </summary>
    Task<IReadOnlyList<ChatMessage>> GetConversationAsync(
        string user1Id,
        string user2Id,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Cuenta mensajes no leídos para un receptor.
    /// </summary>
    Task<int> GetUnreadCountAsync(string receiverId, CancellationToken cancellationToken = default);
}
