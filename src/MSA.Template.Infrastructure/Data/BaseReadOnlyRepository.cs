using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Interfaces;

namespace MSA.Template.Infrastructure.Data;

public class BaseReadOnlyRepository<TEntity, TId> : IReadOnlyRepository<TEntity, TId>
    where TEntity : BaseEntity<TId>
    where TId : IEquatable<TId>
{
    private readonly SlaveDbContext _dbContext;

    public BaseReadOnlyRepository(SlaveDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<TEntity>> FindByAsync(ISpecification<TEntity> specification)
    {
        return await ApplySpecification(specification).AsNoTracking().ToListAsync();
    }

    public async Task<TEntity?> FindByIdAsync(TId id)
    {
        return await _dbContext.Set<TEntity>().Where(s => s.Id.Equals(id)).AsNoTracking().SingleOrDefaultAsync();
    }

    private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec)
    {
        return SpecificationEvaluator<TEntity, TId>.GetQuery(_dbContext.Set<TEntity>().AsQueryable(), spec);
    }
}