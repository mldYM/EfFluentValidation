using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace FluentValidation.EntityFramework
{
    public static class DbContextValidator
    {
        #region TryValidateSignature

        /// <summary>
        /// Validates a <see cref="DbContext"/> an relies on the caller to handle those results.
        /// </summary>
        /// <param name="dbContext">
        /// The <see cref="DbContext"/> to validate.
        /// </param>
        /// <param name="validatorFactory">
        /// A factory that accepts a entity type and returns
        /// a list of corresponding <see cref="IValidator"/>.
        /// </param>
        public static async Task<(bool isValid, IReadOnlyList<EntityValidationFailure> failures)> TryValidate(
                DbContext dbContext,
                Func<Type, IEnumerable<IValidator>> validatorFactory)

            #endregion

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
                var efContext = new EfContext(dbContext, entry);
                validationContext.RootContextData.Add("EfContext", efContext);
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

            return (!entityFailures.Any(), entityFailures);
        }

        #region ValidateSignature

        /// <summary>
        /// Validates a <see cref="DbContext"/> and throws a <see cref="MessageValidationException"/>
        /// if any changed entity is not valid.
        /// </summary>
        /// <param name="dbContext">
        /// The <see cref="DbContext"/> to validate.
        /// </param>
        /// <param name="validatorFactory">
        /// A factory that accepts a entity type and returns a
        /// list of corresponding <see cref="IValidator"/>.
        /// </param>
        public static async Task Validate(
                DbContext dbContext,
                Func<Type, IEnumerable<IValidator>> validatorFactory)

            #endregion

        {
            var (isValid, failures) = await TryValidate(dbContext, validatorFactory);
            if (!isValid)
            {
                throw new EntityValidationException(failures);
            }
        }
    }
}