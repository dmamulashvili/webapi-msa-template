using SharedKernel;
using System;

namespace IntegrationEvents;

public class OrderPaymentSucceededIntegrationEvent : BaseIntegrationEvent
{
    public OrderPaymentSucceededIntegrationEvent(Guid correlationId, Guid orderId)
    {
        CorrelationId = correlationId;
        OrderId = orderId;
    }

    public Guid OrderId { get; private set; }
}