using EComerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EComerce.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de EF Core para la entidad ChatMessage.
/// </summary>
public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.SenderId)
            .IsRequired()
            .HasMaxLength(450); // longitud de IdentityUser.Id

        builder.Property(m => m.ReceiverId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(m => m.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(m => m.CreatedAt)
            .IsRequired();

        // Índice para consultas de conversación eficientes
        builder.HasIndex(m => new { m.SenderId, m.ReceiverId });
        builder.HasIndex(m => new { m.ReceiverId, m.IsRead }); // para contar no leídos
    }
}
