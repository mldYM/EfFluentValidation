using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            .Where(x => x.State == EntityState.Added ||
                        x.State == EntityState.Modified);
    }

    public static async Task<List<T>> ToAsyncList<T>(this IAsyncEnumerable<T> enumerable)
    {
        var list = new List<T>();
        await foreach (var item in enumerable)
        {
            list.Add(item);
        }

        return list;
    }

    public static IEnumerable<string> ChangedProperties(this EntityEntry entry)
    {
        return entry.Properties
            .Where(x =>
            {
                if (!x.IsModified)
                {
                    return false;
                }

                var original = x.OriginalValue;
                var current = x.CurrentValue;
                if (ReferenceEquals(original, current))
                {
                    return false;
                }

                if (original == null)
                {
                    return true;
                }

                return !original.Equals(current);
            })
            .Select(x=>x.Metadata.Name);
    }
}