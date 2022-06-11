using MSA.Template.SharedKernel;

namespace MSA.Template.Core.OrderAggregate.Events;

public class OrderPaidDomainEvent : BaseDomainEvent
{
    public OrderPaidDomainEvent(Order order)
    {
        Order = order;
    }

    public Order Order { get; private set; }
}