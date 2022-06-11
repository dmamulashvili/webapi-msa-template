using System;

namespace MSA.Template.SharedKernel.IntegrationEvents;

public interface IIntegrationEvent
{
    Guid CorrelationId { get; }

    Guid InitiatorId { get; }

    void SetInitiator(Guid initiatorId);
}