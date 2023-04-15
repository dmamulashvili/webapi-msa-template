using System.Threading.Tasks;
using Ardalis.GuardClauses;
using SharedKernel;
using SharedKernel.Interfaces;

namespace MSA.Template.Infrastructure.Data;

public class BaseRepository<TEntity, TId> : IRepository<TEntity, TId> where TEntity : BaseEntity<TId>, IAggregateRoot
{
    private readonly MasterDbContext _dbContext;

    public BaseRepository(MasterDbContext dbContext)
    {
        _dbContext = Guard.Against.Null(dbContext);
    }

    public IUnitOfWork UnitOfWork => _dbContext;

    public async Task AddAsync(TEntity entity)
    {
        await _dbContext.AddAsync(entity);
    }

    public async Task<TEntity?> FindByIdAsync(TId id)
    {
        return await _dbContext.FindAsync<TEntity>(id);
    }

    public void Update(TEntity entity)
    {
        _dbContext.Update(entity);
    }
}