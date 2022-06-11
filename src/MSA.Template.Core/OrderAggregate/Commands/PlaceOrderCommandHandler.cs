using Microsoft.Extensions.Logging;
using MSA.Template.SharedKernel;
using MSA.Template.SharedKernel.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MSA.Template.Core.OrderAggregate.Commands;

public class PlaceOrderCommandHandler : BaseCommandHandler<PlaceOrderCommand, bool>
{
    private readonly IRepository<Order, Guid> _repository;

    private readonly IIdentityService _identityService;

    private readonly ILogger<PlaceOrderCommandHandler> _logger;

    public PlaceOrderCommandHandler(IRepository<Order, Guid> repository,
        IIdentityService identityService,
        ILogger<PlaceOrderCommandHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<bool> Handle(PlaceOrderCommand command, CancellationToken cancellationToken)
    {
        var userId = _identityService.GetUserIdentity();
        var address = new Address(command.City, command.Street);
        var order = Order.CreateNewDraft(userId, address);

        foreach (var item in command.OrderItems)
        {
            order.AddOrderItem(item.ItemId, 1, item.Quantity);
        }

        order.MarkAsPlaced();

        _logger.LogInformation("----- Placing Order - Order: {@Order}", order);

        await _repository.AddAsync(order);

        return await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}