using FluentValidation;

namespace MSA.Template.Core.OrderAggregate.Commands;

public class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty();
    }
}