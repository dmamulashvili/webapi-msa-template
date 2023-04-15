using System.Threading.Tasks;

namespace SharedKernel.Interfaces;

public interface IRepository<TEntity, in TId>
    where TEntity : IAggregateRoot
{
    IUnitOfWork UnitOfWork { get; }

    Task AddAsync(TEntity entity);

    Task<TEntity?> FindByIdAsync(TId id);

    void Update(TEntity entity);
}
