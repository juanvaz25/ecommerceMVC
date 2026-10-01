namespace EComerce.Domain.Common;

/// <summary>
/// Clase base para todas las entidades de dominio.
/// Provee identidad y auditoría básica.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }

    public void SetUpdatedAt() => UpdatedAt = DateTime.UtcNow;
}
