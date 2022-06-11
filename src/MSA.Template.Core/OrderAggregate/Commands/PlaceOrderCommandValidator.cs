using System.Collections.Generic;
using System.Linq;
using FluentValidation;

namespace MSA.Template.Core.OrderAggregate.Commands;

public class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
{
    public PlaceOrderCommandValidator()
    {
        RuleFor(command => command.City).NotEmpty();
        RuleFor(command => command.Street).NotEmpty();
        RuleFor(command => command.OrderItems).NotEmpty().WithMessage("Order items required");
        RuleForEach(command => command.OrderItems).SetValidator(new OrderItemValidator());
    }
    
    private class OrderItemValidator : AbstractValidator<PlaceOrderCommand.OrderItemDTO>
    {
        public OrderItemValidator()
        {
            RuleFor(dto => dto.ItemId).GreaterThan(0);
            RuleFor(dto => dto.Quantity).GreaterThan(0);
        }
    }
}