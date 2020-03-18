using System;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.FluentValidation
{
    public class EfContext
    {
        public DbContext DbContext { get; }
        public Type EntityType { get; }

        public EfContext(DbContext dbContext, Type entityType)
        {
            Guard.AgainstNull(dbContext, nameof(dbContext));
            Guard.AgainstNull(entityType, nameof(entityType));
            DbContext = dbContext;
            EntityType = entityType;
        }
    }
}