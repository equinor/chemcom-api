using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.Interceptors;

public class AuditableEntitiesInterceptor : SaveChangesInterceptor
{
    public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        DbContext dbContext = eventData.Context;
        if (dbContext is not null)
        {           
            IEnumerable<EntityEntry<IAuditable>> entries = dbContext.ChangeTracker.Entries<IAuditable>();

            foreach (EntityEntry<IAuditable> entry in entries)
            {
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    entry.Property(x => x.Updated).CurrentValue = DateTime.Now;
                }
            }
        }

        return base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}
