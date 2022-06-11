using FluentValidation;

namespace MSA.Template.Core.OrderAggregate.Commands;

public class MarkOrderAsPaidCommandValidator : AbstractValidator<MarkOrderAsPaidCommand>
{
    public MarkOrderAsPaidCommandValidator()
    {
        RuleFor(o => o.Id).NotEmpty();
    }
}