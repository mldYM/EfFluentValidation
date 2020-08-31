using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

static class AsyncValidatorChecker
{
    public static bool IsAsync(IValidator validator, IValidationContext context)
    {
        if (validator is IEnumerable<IValidationRule> rules)
        {
            context.RootContextData["__FV_IsAsyncExecution"] = true;
            return rules.Any(validationRule => IsAsync(validationRule, context));
        }

        return false;
    }

    static bool IsAsync(IValidationRule validationRule, IValidationContext context)
    {
        return validationRule.Validators.Any(x => x.ShouldValidateAsynchronously(context));
    }

    public static Task<ValidationResult> ValidateEx(this IValidator validator, IValidationContext validationContext)
    {
        if (IsAsync(validator, validationContext))
        {
            return validator.ValidateAsync(validationContext);
        }

        return Task.FromResult(validator.Validate(validationContext));
    }
}