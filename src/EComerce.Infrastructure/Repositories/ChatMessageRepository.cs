using EComerce.Domain.Entities;
using EComerce.Domain.Interfaces;
using EComerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EComerce.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de mensajes de chat.
/// </summary>
public class ChatMessageRepository : Repository<ChatMessage>, IChatMessageRepository
{
    public ChatMessageRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ChatMessage>> GetConversationAsync(
        string user1Id,
        string user2Id,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(m =>
                (m.SenderId == user1Id && m.ReceiverId == user2Id) ||
                (m.SenderId == user2Id && m.ReceiverId == user1Id))
            .OrderBy(m => m.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetUnreadCountAsync(
        string receiverId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .CountAsync(m => m.ReceiverId == receiverId && !m.IsRead, cancellationToken);
    }
}
