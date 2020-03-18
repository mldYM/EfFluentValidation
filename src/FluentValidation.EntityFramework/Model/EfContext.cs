using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FluentValidation.EntityFramework
{
    #region EfContext

    public class EfContext
    {
        public DbContext DbContext { get; }
        public EntityEntry EntityEntry { get; }

        #endregion

        public EfContext(DbContext dbContext, EntityEntry entityEntry)
        {
            Guard.AgainstNull(dbContext, nameof(dbContext));
            Guard.AgainstNull(entityEntry, nameof(entityEntry));
            DbContext = dbContext;
            EntityEntry = entityEntry;
        }
    }
}