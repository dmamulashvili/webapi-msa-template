using Microsoft.Extensions.Logging;
using SharedKernel;
using SharedKernel.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;

namespace MSA.Template.Core.OrderAggregate.Commands;

public class PlaceOrderCommandHandler : BaseCommandHandler<PlaceOrderCommand, string>
{
    private readonly IRepository<Order, Guid> _repository;

    private readonly ILogger<PlaceOrderCommandHandler> _logger;

    public PlaceOrderCommandHandler(IRepository<Order, Guid> repository,
        ILogger<PlaceOrderCommandHandler> logger)
    {
        _repository = Guard.Against.Null(repository);
        _logger = Guard.Against.Null(logger);
    }

    public override async Task<string> Handle(PlaceOrderCommand command, CancellationToken cancellationToken)
    {
        var address = new Address(command.City, command.Street);
        var order = Order.CreateNewDraft(address);

        foreach (var item in command.OrderLines)
        {
            var orderLine = new OrderLine(item.ItemId, 1, item.Quantity);
            order.AddOrderLine(orderLine);
        }

        order.MarkAsPlaced();

        _logger.LogInformation("----- Placing Order - Order: {@Order}", order);

        await _repository.AddAsync(order);

        await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return order.Id.ToString();
    }
}