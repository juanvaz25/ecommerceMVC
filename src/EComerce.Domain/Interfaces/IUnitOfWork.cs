namespace EComerce.Domain.Interfaces;

/// <summary>
/// Unidad de Trabajo para persistir cambios de múltiples repositorios atómicamente.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IChatMessageRepository ChatMessages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
