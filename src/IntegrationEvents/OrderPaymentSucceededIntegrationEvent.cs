using SharedKernel;
using System;

namespace IntegrationEvents;

public class OrderPaymentSucceededIntegrationEvent : BaseIntegrationEvent
{
    public OrderPaymentSucceededIntegrationEvent(Guid orderId)
    {
        OrderId = orderId;
    }

    public Guid OrderId { get; private set; }
}