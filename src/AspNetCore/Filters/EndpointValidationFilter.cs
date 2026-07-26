using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aurore.Foundation.AspNetCore.Extensions;
using Aurore.Foundation.Core.Validation;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace Aurore.Foundation.AspNetCore.Filters;

internal sealed class EndpointValidationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var errors = new List<ValidationFailure>();

        foreach (var argument in context.Arguments)
        {
            if (argument is null)
                continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());

            if (context.HttpContext.RequestServices.GetService(validatorType) is not IValidator validator)
                continue;

            var contextType = typeof(ValidationContext<>).MakeGenericType(argument.GetType());

            var validationContext = Activator.CreateInstance(contextType, argument) as IValidationContext;

            var result = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);

            if (result.IsValid)
                continue;

            errors.AddRange(result.Errors);
        }

        if (errors.Count == 0)
            return await next(context);

        return TypedResults.ValidationError(errors.NormalizeErrors());
    }
}
