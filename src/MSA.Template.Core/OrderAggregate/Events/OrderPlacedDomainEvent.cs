using MSA.Template.SharedKernel;

namespace MSA.Template.Core.OrderAggregate.Events;

public class OrderPlacedDomainEvent : BaseDomainEvent
{
    public OrderPlacedDomainEvent(Order order)
    {
        Order = order;
    }

    public Order Order { get; private set; }
}