using EComerce.Domain.Interfaces;
using EComerce.Infrastructure.Persistence;
using EComerce.Infrastructure.Repositories;

namespace EComerce.Infrastructure.Persistence;

/// <summary>
/// Implementación de la Unidad de Trabajo.
/// Centraliza el guardado y los repositorios.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IChatMessageRepository? _chatMessages;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IChatMessageRepository ChatMessages =>
        _chatMessages ??= new ChatMessageRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public void Dispose() => _context.Dispose();
}
