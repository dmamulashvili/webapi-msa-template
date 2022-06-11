using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MSA.Template.Core;
using MSA.Template.Core.OrderAggregate.Commands;
using MSA.Template.SharedKernel.Extensions;

namespace MSA.Template.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [Route("place")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> PlaceOrderAsync([FromBody] PlaceOrderCommand command, [FromHeader(Name = "x-requestid")] string requestId)
    {
        bool commandResult = false;

        if (Guid.TryParse(requestId, out Guid correlationId) && correlationId != Guid.Empty)
        {
            var requestPlaceOrder = new IdentifiedCommand<PlaceOrderCommand, bool>(command, correlationId);

            _logger.LogInformation(
                "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                requestPlaceOrder.GetGenericTypeName(),
                nameof(requestPlaceOrder.CorrelationId),
                requestPlaceOrder.CorrelationId,
                requestPlaceOrder);

            commandResult = await _mediator.Send(requestPlaceOrder);
        }

        if (!commandResult)
        {
            return BadRequest();
        }

        return Ok();
    }
}