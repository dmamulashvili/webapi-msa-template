using SharedKernel;
using System;

namespace IntegrationEvents;

public class OrderPaidIntegrationEvent : BaseIntegrationEvent
{
    public OrderPaidIntegrationEvent(Guid correlationId, Guid orderId)
    {
        CorrelationId = correlationId;
        OrderId = orderId;
    }

    public Guid OrderId { get; private set; }
}