using SharedKernel.IntegrationEvents;
using System;

namespace SharedKernel;

public class BaseIntegrationEvent : IIntegrationEvent
{
    // public Guid CorrelationId { get; protected init; }
    //
    // public Guid InitiatorId { get; private set; }
    //
    // void IIntegrationEvent.SetInitiator(Guid initiatorId) => InitiatorId = initiatorId;
}