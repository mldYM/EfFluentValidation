using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

static class Extensions
{
    public static IEnumerable<EntityEntry> ModifiedEntries(this DbContext dbContext)
    {
        dbContext.ChangeTracker.DetectChanges();
        return dbContext.ChangeTracker.Entries().Where(x=>x.State == EntityState.Added || x.State == EntityState.Modified);
    }
}