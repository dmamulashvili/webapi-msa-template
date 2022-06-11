using MSA.Template.SharedKernel.IntegrationEvents;
using System;

namespace MSA.Template.SharedKernel;

public class BaseIntegrationEvent : IIntegrationEvent
{
    public Guid CorrelationId { get; protected init; }

    public Guid InitiatorId { get; private set; }

    void IIntegrationEvent.SetInitiator(Guid initiatorId) => InitiatorId = initiatorId;
}