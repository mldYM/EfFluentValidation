using System.Diagnostics;
using EfFluentValidation;
using FluentValidation;

public class ValidatorWithContext :
    AbstractValidator<Employee>
{
    public ValidatorWithContext()
    {
        RuleFor(_ => _.Content)
            .Custom((propertyValue, validationContext) =>
            {
                var efContext = validationContext.EfContext();
                Debug.Assert(efContext.DbContext != null);
                Debug.Assert(efContext.EntityEntry != null);

                if (propertyValue == "BadValue")
                {
                    validationContext.AddFailure("BadValue");
                }
            });
    }
}