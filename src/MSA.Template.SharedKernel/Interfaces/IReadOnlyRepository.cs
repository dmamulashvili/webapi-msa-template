using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSA.Template.SharedKernel.Interfaces;

public interface IReadOnlyRepository<TEntity, in TId>
    where TEntity : BaseEntity<TId>
    where TId : IEquatable<TId>
{
    Task<IEnumerable<TEntity>> FindByAsync(ISpecification<TEntity> specification);

    Task<TEntity?> FindByIdAsync(TId id);
}