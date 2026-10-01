using EComerce.Domain.Common;

namespace EComerce.Domain.Entities;

/// <summary>
/// Entidad de mensaje de chat entre dos usuarios.
/// </summary>
public class ChatMessage : BaseEntity
{
    public string SenderId { get; private set; } = null!;
    public string ReceiverId { get; private set; } = null!;
    public string Content { get; private set; } = null!;
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }

    // Constructor para EF Core
    private ChatMessage() { }

    public static ChatMessage Create(string senderId, string receiverId, string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(senderId, nameof(senderId));
        ArgumentException.ThrowIfNullOrWhiteSpace(receiverId, nameof(receiverId));
        ArgumentException.ThrowIfNullOrWhiteSpace(content, nameof(content));

        return new ChatMessage
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Content = content,
            IsRead = false
        };
    }

    public void MarkAsRead()
    {
        if (!IsRead)
        {
            IsRead = true;
            ReadAt = DateTime.UtcNow;
            SetUpdatedAt();
        }
    }
}
