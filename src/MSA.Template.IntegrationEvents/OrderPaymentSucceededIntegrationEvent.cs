using MSA.Template.SharedKernel;
using System;

namespace MSA.Template.IntegrationEvents;

public class OrderPaymentSucceededIntegrationEvent : BaseIntegrationEvent
{
    public OrderPaymentSucceededIntegrationEvent(Guid correlationId, Guid orderId)
    {
        CorrelationId = correlationId;
        OrderId = orderId;
    }

    public Guid OrderId { get; private set; }
}