using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.FluentValidation
{
    public static class DbContextValidator
    {
        public static async Task Validate(DbContext dbContext, Func<Type, IEnumerable<IValidator>> validatorFactory)
        {
            Guard.AgainstNull(dbContext, nameof(dbContext));
            Guard.AgainstNull(validatorFactory, nameof(validatorFactory));
            dbContext.ChangeTracker.DetectChanges();
            var entries = dbContext.ChangeTracker.Entries();
            var entityFailures = new List<EntityValidationFailure>();
            foreach (var entry in entries)
            {
                var validationFailuresFailures = new List<TypeValidationFailure>();
                var entity = entry.Entity;
                var validationContext = new ValidationContext(entity);
                var clrType = entry.Metadata.ClrType;
                validationContext.RootContextData.Add("EfContext", new EfContext(dbContext,clrType));
                foreach (var validator in validatorFactory(clrType))
                {
                    IList<ValidationFailure> errors;
                    if (AsyncValidatorChecker.IsAsync(validator, validationContext))
                    {
                        var result = await validator.ValidateAsync(validationContext);
                        errors = result.Errors;
                    }
                    else
                    {
                        var result = validator.Validate(validationContext);
                        errors = result.Errors;
                    }

                    validationFailuresFailures.AddRange(errors.Select(failure => new TypeValidationFailure(validator.GetType(), failure)));
                }

                if (validationFailuresFailures.Any())
                {
                    entityFailures.Add(new EntityValidationFailure(entity, clrType, validationFailuresFailures));
                }
            }

            if (entityFailures.Any())
            {
                throw new EntityValidationException(entityFailures);
            }
        }
    }
}