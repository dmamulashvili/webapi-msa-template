using System.Net;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MSA.Template.Core;
using MSA.Template.Core.OrderAggregate.Commands;
using SharedKernel.Extensions;

namespace MSA.Template.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
    {
        _mediator = Guard.Against.Null(mediator);
        _logger = Guard.Against.Null(logger);
    }

    [HttpPost("place")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> PlaceOrderAsync(
        [FromBody] PlaceOrderCommand command,
        [FromHeader(Name = "x-request-id")] string requestId
    )
    {
        var commandResult = string.Empty;

        if (Guid.TryParse(requestId, out Guid correlationId) && correlationId != Guid.Empty)
        {
            var identifiedCommand = new IdentifiedCommand<PlaceOrderCommand, string>(
                command,
                correlationId
            );

            _logger.LogInformation(
                "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                identifiedCommand.GetGenericTypeName(),
                nameof(identifiedCommand.CorrelationId),
                identifiedCommand.CorrelationId,
                identifiedCommand
            );

            commandResult = await _mediator.Send(identifiedCommand);
        }

        if (string.IsNullOrWhiteSpace(commandResult))
        {
            return BadRequest();
        }

        return Ok(commandResult);
    }

    [HttpPut("cancel")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CancelOrderAsync(
        [FromBody] CancelOrderCommand command,
        [FromHeader(Name = "x-request-id")] string requestId
    )
    {
        bool commandResult = false;

        if (Guid.TryParse(requestId, out Guid correlationId) && correlationId != Guid.Empty)
        {
            var identifiedCommand = new IdentifiedCommand<CancelOrderCommand, bool>(
                command,
                correlationId
            );

            _logger.LogInformation(
                "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                identifiedCommand.GetGenericTypeName(),
                nameof(identifiedCommand.CorrelationId),
                identifiedCommand.CorrelationId,
                identifiedCommand
            );

            commandResult = await _mediator.Send(identifiedCommand);
        }

        if (!commandResult)
        {
            return BadRequest();
        }

        return Ok();
    }
}
