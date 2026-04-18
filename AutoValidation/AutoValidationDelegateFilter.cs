using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace AutoValidation;

public class AutoValidationDelegateFilter : IAsyncActionFilter
{
    private static readonly ConcurrentDictionary<Type, Func<object, object, CancellationToken, Task<ValidationResult>>> _validateDelegates = new();

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var values = context.ActionArguments.Values.Where(v => v != null && v.GetType().IsDefined(typeof(AutoValidationAttribute), true));
        if (context.ActionDescriptor is not ControllerActionDescriptor || !values.Any())
        {
            await next();
            return;
        }

        var services = context.HttpContext.RequestServices;
        var cancellation = context.HttpContext.RequestAborted;

        foreach (var model in values)
        {
            if (model is null) continue;

            var modelType = model.GetType();
            var validatorType = typeof(IValidator<>).MakeGenericType(modelType);
            var validator = services.GetService(validatorType);

            if (validator is null) continue;

            var validate = _validateDelegates.GetOrAdd(modelType, CreateValidateDelegate);

            var result = await validate(validator, model, cancellation);

            if (!result.IsValid)
                throw new ValidationException(result.Errors);
        }

        await next();
    }

    private static Func<object, object, CancellationToken, Task<ValidationResult>> CreateValidateDelegate(Type modelType)
    {
        var validatorType = typeof(IValidator<>).MakeGenericType(modelType);

        var validatorObj = Expression.Parameter(typeof(object), "validator");
        var modelObj = Expression.Parameter(typeof(object), "model");
        var cancellation = Expression.Parameter(typeof(CancellationToken), "ct");

        var validatorCast = Expression.Convert(validatorObj, validatorType);
        var modelCast = Expression.Convert(modelObj, modelType);

        var validateMethod = validatorType.GetMethod(nameof(IValidator<>.ValidateAsync), [modelType, typeof(CancellationToken)])!;

        var call = Expression.Call(
            validatorCast,
            validateMethod,
            modelCast,
            cancellation
        );

        return Expression.Lambda<Func<object, object, CancellationToken, Task<ValidationResult>>>(
                call,
                validatorObj,
                modelObj,
                cancellation
            )
            .Compile();
    }
}
