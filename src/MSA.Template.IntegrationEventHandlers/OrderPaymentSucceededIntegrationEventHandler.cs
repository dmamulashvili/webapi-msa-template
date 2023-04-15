using Ardalis.GuardClauses;
using IntegrationEvents;
using MassTransit;
using MediatR;
using MSA.Template.Core;
using MSA.Template.Core.OrderAggregate.Commands;
using SharedKernel.IntegrationEvents;

namespace MSA.Template.IntegrationEventHandlers;

public class OrderPaymentSucceededIntegrationEventHandler
    : IIntegrationEventHandler<OrderPaymentSucceededIntegrationEvent>
{
    private readonly IMediator _mediator;

    public OrderPaymentSucceededIntegrationEventHandler(IMediator mediator)
    {
        _mediator = Guard.Against.Null(mediator);
    }

    public Task Consume(ConsumeContext<OrderPaymentSucceededIntegrationEvent> context)
    {
        Guard.Against.Null(context.CorrelationId);

        var @event = context.Message;

        return _mediator.Publish(
            new IdentifiedCommand<MarkOrderAsPaidCommand, bool>(
                new MarkOrderAsPaidCommand(@event.OrderId),
                context.CorrelationId.Value
            ),
            context.CancellationToken
        );
    }
}
