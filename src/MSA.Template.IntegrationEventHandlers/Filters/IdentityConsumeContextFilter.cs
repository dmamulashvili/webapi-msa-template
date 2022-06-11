using Ardalis.GuardClauses;
using MassTransit;
using MSA.Template.SharedKernel.IntegrationEvents;
using MSA.Template.SharedKernel.Interfaces;

namespace MSA.Template.IntegrationEventHandlers.Filters;

public class IdentityConsumeContextFilter<T> : IFilter<ConsumeContext<T>> where T : class
{
    private readonly IIdentityServiceProvider _identityServiceProvider;

    public IdentityConsumeContextFilter(IIdentityServiceProvider identityServiceProvider)
    {
        _identityServiceProvider = identityServiceProvider;
    }

    public void Probe(ProbeContext context)
    {
    }

    public Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        Guard.Against.Null(context.InitiatorId, nameof(context.InitiatorId));

        if (context.Message is IIntegrationEvent @event)
        {
            @event.SetIdentity(context.InitiatorId.Value);
            _identityServiceProvider.SetIdentity(@event.UserIdentity);
        }

        return next.Send(context);
    }
}