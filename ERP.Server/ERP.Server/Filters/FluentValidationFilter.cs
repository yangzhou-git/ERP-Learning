using ERP.Server.Common;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ERP.Server.Filters
{
    public class FluentValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument == null)
                {
                    continue;
                }
                var requestType = argument.GetType();
                var validatorType = typeof(IValidator<>).MakeGenericType(requestType);
                var validator = context.HttpContext.RequestServices.GetService(validatorType) as IValidator;
                if (validator == null)
                {
                    continue;
                }
                var validationContext = new ValidationContext<object>(argument);
                var result = await validator.ValidateAsync(validationContext);
                if (!result.IsValid)
                {
                    context.Result = new BadRequestObjectResult(ApiResponse<bool>.Error(result.Errors[0].ErrorMessage));
                    return;
                }
            }
                await next();
        }
    }
}