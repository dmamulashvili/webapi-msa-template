using IntegrationEvents;
using MSA.Template.Core.OrderAggregate.Events;
using SharedKernel;
using SharedKernel.IntegrationEvents;
using SharedKernel.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MSA.Template.Core.OrderAggregate.EventHandlers;

public class OrderPlacedDomainEventHandler : BaseDomainEventHandler<OrderPlacedDomainEvent>
{
    private readonly IRepository<Order, Guid> _repository;
    private readonly IIntegrationEventService _integrationEventService;

    public OrderPlacedDomainEventHandler(
        IIntegrationEventService integrationEventService, IRepository<Order, Guid> repository)
    {
        _integrationEventService =
            integrationEventService ?? throw new ArgumentNullException(nameof(integrationEventService));
        _repository = repository;
    }

    // INFO: e.g. publish fot Notification API to notify customer about succeeded order placement.
    public override async Task Handle(OrderPlacedDomainEvent @event, CancellationToken cancellationToken)
    {
        await _repository.UnitOfWork.SaveCorrelationAsync(cancellationToken);
        
        var orderPlacedIntegrationEvent = new OrderPlacedIntegrationEvent(
            @event.Order.CorrelationId,
            @event.Order.Id,
            @event.Order.OrderDate,
            @event.Order.OrderLines.Select(x =>
                new OrderPlacedIntegrationEvent.OrderLine(
                    x.ItemId,
                    x.ItemPrice,
                    x.Quantity)).ToList());

        await _integrationEventService.AddEventAsync(orderPlacedIntegrationEvent);
    }
}