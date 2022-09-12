using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharedKernel.Exceptions;

namespace MSA.Template.API.Filters;

public class DomainExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is DomainException)
        {
            if (context.Exception.InnerException is ValidationException validationException)
            {
                context.Result = new BadRequestObjectResult(validationException.Errors);
            }
            else
            {
                context.Result = new UnprocessableEntityObjectResult(context.Exception.Message);
            }
        }
    }
}