using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace SharedKernel.Interfaces;

public interface IUnitOfWork : IDisposable
{
    bool HasActiveTransaction { get; }

    IExecutionStrategy CreateExecutionStrategy(Guid correlationId);
    Task<IDbContextTransaction?> BeginTransactionAsync(CancellationToken cancellationToken);
    Task CommitTransactionAsync(IDbContextTransaction? transaction);
    Task RollbackTransaction();

    Task SaveCorrelationAsync(CancellationToken cancellationToken = default(CancellationToken));
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

    Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken));
}
