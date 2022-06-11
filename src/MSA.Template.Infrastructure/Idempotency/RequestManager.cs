using System;
using System.Threading.Tasks;
using MSA.Template.SharedKernel.Exceptions;
using MSA.Template.SharedKernel.Interfaces;

namespace MSA.Template.Infrastructure.Idempotency;

public class RequestManager : IRequestManager
{
    private readonly MasterDbContext _dbContext;

    public RequestManager(MasterDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }


    public async Task<bool> ExistAsync(Guid id)
    {
        var request = await _dbContext.
            FindAsync<ClientRequest>(id);

        return request != null;
    }

    public async Task CreateRequestForCommandAsync<T>(Guid id)
    {
        var exists = await ExistAsync(id);

        var request = exists ?
            throw new DomainException($"Request with {id} already exists") :
            new ClientRequest()
            {
                Id = id,
                Name = typeof(T).Name,
                Time = DateTime.UtcNow
            };

        _dbContext.Add(request);

        await _dbContext.SaveChangesAsync();
    }
}