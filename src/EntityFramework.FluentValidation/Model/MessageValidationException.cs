using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFramework.FluentValidation
{
    public class EntityValidationException :
        Exception
    {
        public EntityValidationException(IReadOnlyList<EntityValidationFailure> errors)
        {
            Guard.AgainstNull(errors, nameof(errors));
            Errors = errors;
        }

        public override string Message
        {
            get
            {
                var builder = new StringBuilder("Validation failed");
                builder.AppendLine();
                foreach (var error in Errors)
                {
                    builder.AppendLine($"  * {error.EntityType.FullName}");
                    foreach (var errorFailure in error.Failures)
                    {
                        var failure = errorFailure.Failure;
                        builder.AppendLine($"   * {failure.PropertyName}: {failure.ErrorMessage} (Validator: {errorFailure.ValidatorType.FullName})");
                    }
                    builder.AppendLine();
                }

                return builder.ToString();
            }
        }

        public IReadOnlyList<EntityValidationFailure> Errors { get; }
    }
}