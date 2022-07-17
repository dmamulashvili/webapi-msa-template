using MassTransit;
using MediatR;
using MSA.Template.Core;
using MSA.Template.Core.OrderAggregate.Commands;
using MSA.Template.IntegrationEvents;
using SharedKernel.IntegrationEvents;

namespace MSA.Template.IntegrationEventHandlers;

public class OrderPaymentSucceededIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentSucceededIntegrationEvent>
{
    private readonly IMediator _mediator;

    public OrderPaymentSucceededIntegrationEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task Consume(ConsumeContext<OrderPaymentSucceededIntegrationEvent> context)
    {
        var message = context.Message;

        return _mediator.Publish(
            new IdentifiedCommand<MarkOrderAsPaidCommand, bool>(new MarkOrderAsPaidCommand(message.OrderId),
                message.CorrelationId), context.CancellationToken);
    }
}