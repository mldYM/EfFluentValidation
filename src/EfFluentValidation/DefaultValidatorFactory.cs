using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace EfFluentValidation
{
    public static class DefaultValidatorFactory<T>
        where T : DbContext
    {
        public static Func<Type, IEnumerable<IValidator>> Factory { get; }

        static DefaultValidatorFactory()
        {
            var validators = ValidationFinder.FromAssemblyContaining<T>();

            var typeCache = new ValidatorTypeCache(validators);
            Factory = type =>
            {
                if (typeCache.TryGetValidators(type, out var enumerable))
                {
                    return enumerable;
                }

                return Enumerable.Empty<IValidator>();
            };
        }
    }
}