using Ardalis.GuardClauses;
using MassTransit;
using SharedKernel.IntegrationEvents;
using SharedKernel.Interfaces;

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
            _identityServiceProvider.SetUserIdentity(context.InitiatorId.Value);
        }

        return next.Send(context);
    }
}