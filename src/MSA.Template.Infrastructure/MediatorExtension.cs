using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SharedKernel.Interfaces;

namespace MSA.Template.Infrastructure;

static class MediatorExtension
{
    public static async Task DispatchDomainEventsAsync(this IMediator mediator, MasterDbContext masterDbContext)
    {
        var domainEntities = masterDbContext.ChangeTracker
            .Entries<IEntity>()
            .Where(e => e.Entity.DomainEvents != null && e.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(e => e.Entity.DomainEvents!)
            .ToList();

        domainEntities.ToList()
            .ForEach(e => e.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await mediator.Publish(domainEvent);
    }
}