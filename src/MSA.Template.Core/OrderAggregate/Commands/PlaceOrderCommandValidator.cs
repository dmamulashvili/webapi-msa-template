using FluentValidation;

namespace MSA.Template.Core.OrderAggregate.Commands;

public class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
{
    public PlaceOrderCommandValidator()
    {
        RuleFor(command => command.City).NotEmpty();
        RuleFor(command => command.Street).NotEmpty();
        RuleFor(command => command.OrderLines).NotEmpty().WithMessage("Order lines required");
        RuleForEach(command => command.OrderLines).SetValidator(new OrderLineValidator());
    }

    private class OrderLineValidator : AbstractValidator<PlaceOrderCommand.OrderLineDto>
    {
        public OrderLineValidator()
        {
            RuleFor(dto => dto.ItemId).GreaterThan(0);
            RuleFor(dto => dto.Quantity).GreaterThan(0);
        }
    }
}