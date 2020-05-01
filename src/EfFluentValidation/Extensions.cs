using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

static class Extensions
{
    public static IEnumerable<EntityEntry> ModifiedEntries(this DbContext dbContext)
    {
        var tracker = dbContext.ChangeTracker;
        tracker.DetectChanges();
        return tracker
            .Entries()
            .Where(x=>x.State == EntityState.Added ||
                      x.State == EntityState.Modified);
    }
}