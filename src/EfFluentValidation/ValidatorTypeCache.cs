using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Result = FluentValidation.AssemblyScanner.AssemblyScanResult;

namespace EfFluentValidation
{
    public class ValidatorTypeCache
    {
        ConcurrentDictionary<Type, IEnumerable<IValidator>> entityMapCache = new();
        Dictionary<Type, List<IValidator>> instanceCache = new();

        public ValidatorTypeCache(IEnumerable<Result> scanResults)
        {
            foreach (var result in scanResults.GroupBy(x => x.InterfaceType.GenericTypeArguments.Single()))
            {
                instanceCache[result.Key] = result
                    .Select(x => Activator.CreateInstance(x.ValidatorType))
                    .Cast<IValidator>()
                    .ToList();
            }
        }

        public IEnumerable<IValidator> GetValidators(Type entityType)
        {
            return entityMapCache.GetOrAdd(entityType, x =>
            {
                var list = FindValidatorsForEntity(x);
                if (list.Any())
                {
                    return list;
                }

                return Enumerable.Empty<IValidator>();
            });
        }

        List<IValidator> FindValidatorsForEntity(Type entityType)
        {
            var list = new List<IValidator>();
            foreach (var typeToValidators in instanceCache)
            {
                var targetType = typeToValidators.Key;
                var validators = typeToValidators.Value;
                if (targetType.IsAssignableFrom(entityType))
                {
                    list.AddRange(validators);
                }
            }
            return list;
        }
    }
}