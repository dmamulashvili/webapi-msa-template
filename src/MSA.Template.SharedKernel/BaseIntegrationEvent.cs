using MSA.Template.SharedKernel.IntegrationEvents;
using System;

namespace MSA.Template.SharedKernel;

public class BaseIntegrationEvent : IIntegrationEvent
{
    public Guid CorrelationId { get; protected set; }

    public Guid UserIdentity { get; set; }

    void IIntegrationEvent.SetIdentity(Guid identity) => UserIdentity = identity;
}