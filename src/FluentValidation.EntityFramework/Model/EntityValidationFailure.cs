using System;
using System.Collections.Generic;

namespace FluentValidation.EntityFramework
{
    public class EntityValidationFailure
    {
        public object Entity { get; }
        public Type EntityType { get; }
        public IReadOnlyList<TypeValidationFailure> Failures { get; }

        public EntityValidationFailure(object entity,Type entityType, IReadOnlyList<TypeValidationFailure> failures)
        {
            Guard.AgainstNull(entity, nameof(entity));
            Guard.AgainstNull(entityType, nameof(entityType));
            Guard.AgainstNull(failures, nameof(failures));
            Entity = entity;
            EntityType = entityType;
            Failures = failures;
        }
    }
}