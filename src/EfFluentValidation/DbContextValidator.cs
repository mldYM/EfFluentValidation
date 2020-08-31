using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EfFluentValidation
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
            var entityFailures = new List<EntityValidationFailure>();
            var entries = dbContext.ModifiedEntries();
            foreach (var entry in entries)
            {
                var validationFailures = new List<TypeValidationFailure>();
                var clrType = entry.Metadata.ClrType;
                var validationContext = BuildValidationContext(dbContext, entry);
                foreach (var validator in validatorFactory(clrType))
                {
                    var result = await validator.ValidateEx(validationContext);
                    validationFailures.AddRange(result.Errors.Select(failure => new TypeValidationFailure(validator.GetType(), failure)));
                }

                if (validationFailures.Any())
                {
                    entityFailures.Add(new EntityValidationFailure(entry.Entity, entry.Metadata.ClrType, validationFailures));
                }
            }

            return (!entityFailures.Any(), entityFailures);
        }

        static IValidationContext BuildValidationContext(DbContext dbContext, EntityEntry entry)
        {
            //TODO: cache
            var validationContextType = typeof(ValidationContext<>).MakeGenericType(entry.Metadata.ClrType);
            var validationContext = (IValidationContext) Activator.CreateInstance(validationContextType, entry.Entity);
            validationContext.RootContextData.Add("EfContext", new EfContext(dbContext, entry));
            return validationContext;
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