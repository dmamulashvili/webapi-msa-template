using AuditEvents;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MSA.Template.Infrastructure.EntityConfigurations;
using SharedKernel;
using SharedKernel.Interfaces;
using SharedKernel.Audit.Interfaces;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;

namespace MSA.Template.Infrastructure;

public class MasterDbContext : DbContext, IUnitOfWork
{
    public const string DefaultSchema = "api";

    private readonly IMediator _mediator = null!;
    private readonly IIdentityService _identityService = null!;
    private readonly IAuditEventService _auditEventService = null!;

    private Guid _correlationId;
    private IDbContextTransaction? _currentTransaction;

    public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options)
    {
    }

    public MasterDbContext(DbContextOptions<MasterDbContext> options,
        IMediator mediator,
        IIdentityService identityService,
        IAuditEventService auditEventService) : base(options)
    {
        _mediator = Guard.Against.Null(mediator);
        _identityService = Guard.Against.Null(identityService);
        _auditEventService = Guard.Against.Null(auditEventService);
    }

    public IDbContextTransaction? GetCurrentTransaction() => _currentTransaction;

    public bool HasActiveTransaction => _currentTransaction != null;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClientRequestEntityTypeConfiguration).Assembly);

        // modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }

    public Task SaveCorrelationAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        foreach (var item in base.ChangeTracker.Entries<IAggregateRoot>())
        {
            if (item.State == EntityState.Modified || item.State == EntityState.Added)
            {
                item.Entity.SetCorrelationId(_correlationId);
            }
        }

        return Task.CompletedTask;
    }

    public new async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        var modifiedBy = _identityService.GetUserIdentity();

        foreach (var item in base.ChangeTracker.Entries<IAggregateRoot>())
        {
            if (item.State == EntityState.Modified || item.State == EntityState.Added)
            {
                item.Entity.SetCorrelationId(_correlationId);
                item.Entity.SetModifiedBy(modifiedBy.ToString());
                item.Entity.SetDateModified(DateTimeOffset.UtcNow);

                if (item.State == EntityState.Added)
                {
                    item.Entity.SetCreatedBy(modifiedBy.ToString());
                    item.Entity.SetDateCreated(DateTimeOffset.UtcNow);
                }
            }
        }

        var result = await base.SaveChangesAsync(cancellationToken);
        return result;
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        // Dispatch Domain Events collection. 
        // Choices:
        // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
        // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
        // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
        // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
        await _mediator.DispatchDomainEventsAsync(this);

        var modifiedBy = _identityService.GetUserIdentity();

        foreach (var item in base.ChangeTracker.Entries<IAggregateRoot>())
        {
            if (item.State == EntityState.Modified || item.State == EntityState.Added)
            {
                item.Entity.SetCorrelationId(_correlationId);
                item.Entity.SetModifiedBy(modifiedBy.ToString());
                item.Entity.SetDateModified(DateTimeOffset.UtcNow);

                if (item.State == EntityState.Added)
                {
                    item.Entity.SetCreatedBy(modifiedBy.ToString());
                    item.Entity.SetDateCreated(DateTimeOffset.UtcNow);
                }
            }
        }

        foreach (var item in base.ChangeTracker.Entries<BaseEntity<Guid>>())
        {
            if (item.State == EntityState.Modified)
            {
                foreach (var property in item.OriginalValues.Properties)
                {
                    var original = item.OriginalValues[property];
                    var current = item.CurrentValues[property];

                    if (!Equals(original, current))
                    {
                        var entity = item.Entity;
                        var auditEvent = new EntityPropertyModifiedAuditEvent(_correlationId,
                            entity.GetType().Name,
                            entity.Id.ToString(),
                            property.Name,
                            original is not null ? JsonSerializer.Serialize(original) : null,
                            current is not null ? JsonSerializer.Serialize(current) : null,
                            modifiedBy,
                            DateTimeOffset.UtcNow);

                        await _auditEventService.AddEventAsync(auditEvent);
                    }
                }
            }
        }

        // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
        // performed through the DbContext will be committed
        var result = await base.SaveChangesAsync(cancellationToken);

        return true;
    }

    public IExecutionStrategy CreateExecutionStrategy(Guid correlationId)
    {
        _correlationId = correlationId;
        return Database.CreateExecutionStrategy();
    }

    public async Task<IDbContextTransaction?> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        if (_currentTransaction != null) return null;

        _currentTransaction = await Database.BeginTransactionAsync(cancellationToken);

        return _currentTransaction;
    }

    public async Task CommitTransactionAsync(IDbContextTransaction? transaction)
    {
        if (transaction == null) throw new ArgumentNullException(nameof(transaction));
        if (transaction != _currentTransaction)
            throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

        try
        {
            await SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await RollbackTransaction();
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransaction()
    {
        try
        {
            await _currentTransaction?.RollbackAsync()!;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }
}