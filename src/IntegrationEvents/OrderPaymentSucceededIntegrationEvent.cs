using System;
using SharedKernel.IntegrationEvents;

namespace IntegrationEvents;

public class OrderPaymentSucceededIntegrationEvent : IIntegrationEvent
{
    public OrderPaymentSucceededIntegrationEvent(Guid orderId)
    {
        OrderId = orderId;
    }

    public Guid OrderId { get; private set; }
}
