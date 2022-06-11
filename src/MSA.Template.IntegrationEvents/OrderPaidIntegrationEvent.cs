using MSA.Template.SharedKernel;
using System;

namespace MSA.Template.IntegrationEvents;

public class OrderPaidIntegrationEvent : BaseIntegrationEvent
{
    public OrderPaidIntegrationEvent(Guid correlationId, Guid orderId)
    {
        CorrelationId = correlationId;
        OrderId = orderId;
    }

    public Guid OrderId { get; private set; }
}