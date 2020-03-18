using System;
using FluentValidation.Results;

namespace FluentValidation.EntityFramework
{
    public class TypeValidationFailure
    {
        /// <summary>
        /// The <see cref="Type"/> of the <see cref="IValidator"/> that cause the <see cref="Failure"/>.
        /// </summary>
        public Type ValidatorType { get; }
        public ValidationFailure Failure { get; }

        public TypeValidationFailure(Type validatorType, ValidationFailure failure)
        {
            Guard.AgainstNull(validatorType, nameof(validatorType));
            Guard.AgainstNull(failure, nameof(failure));
            ValidatorType = validatorType;
            Failure = failure;
        }
    }
}