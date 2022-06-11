using System;

namespace MSA.Template.SharedKernel.IntegrationEvents;

public interface IIntegrationEvent
{
    Guid CorrelationId { get; }

    Guid UserIdentity { get; }

    void SetIdentity(Guid identity);
}