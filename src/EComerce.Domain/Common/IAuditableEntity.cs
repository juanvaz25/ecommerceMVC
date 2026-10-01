namespace EComerce.Domain.Common;

/// <summary>
/// Interfaz para entidades con auditoría de creación/modificación por usuario.
/// </summary>
public interface IAuditableEntity
{
    string? CreatedBy { get; }
    string? UpdatedBy { get; }
}
