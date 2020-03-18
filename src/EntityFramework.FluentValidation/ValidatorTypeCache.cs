using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Result = FluentValidation.AssemblyScanner.AssemblyScanResult;

namespace EntityFramework.FluentValidation
{
    public class ValidatorTypeCache
    {
        ConcurrentDictionary<Type, IEnumerable<IValidator>> typeCache = new ConcurrentDictionary<Type, IEnumerable<IValidator>>();

        public ValidatorTypeCache(IEnumerable<Result> scanResults)
        {
            foreach (var result in scanResults.GroupBy(x=>x.InterfaceType.GenericTypeArguments.Single()))
            {
                typeCache[result.Key] = result.Select(x => Activator.CreateInstance(x.ValidatorType)).Cast<IValidator>();
            }
        }

        public bool TryGetValidators(Type entityType, out IEnumerable<IValidator> validators)
        {
            return typeCache.TryGetValue(entityType, out validators);
        }
    }
}