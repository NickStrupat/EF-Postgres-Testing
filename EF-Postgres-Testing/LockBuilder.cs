using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

public sealed class LockBuilder
{
    internal LockBuilder() {}
    
    private readonly List<(String entityName, Func<DbContext, CancellationToken, Task> lockAsync)> locks = new();
    
    public LockBuilder AddLock<T>(Expression<Func<T, Boolean>> predicate) where T : class
    {
        ArgumentNullException.ThrowIfNull(predicate);
        // https://www.postgresql.org/docs/current/explicit-locking.html#LOCKING-ROWS
        // I chose to start the query with the DbSet<T> instead of any IQueryable<T> because an IQueryable<T> might be ordered,
        // and the order of the locks is important to avoid deadlocks. By using the DbSet<T>, the query will always be ordered
        // by the primary key of the table, which means the locks will always be acquired in the same order.
        locks.Add((
            entityName: typeof(T).FullName!,
            lockAsync: async (ctx, ct) =>
                await ctx.Set<T>().IgnoreAutoIncludes().AsNoTracking().Where(predicate).Select(_ => 0).ToListAsync(ct)
                // await ctx.Database.ExecuteSqlRawAsync(
                //     ctx.Set<T>().IgnoreAutoIncludes().AsNoTracking().Where(predicate).Select(_ => 0).ToQueryString(),
                //     ct
                // )
        ));
        return this;
    }
    
    internal async Task ApplyLocksAsync(DbContext context, CancellationToken ct)
    {
        // to avoid deadlocks, order the locks by entity name so locks are acquired in the same order
        // https://www.postgresql.org/docs/current/explicit-locking.html#LOCKING-DEADLOCKS
        locks.Sort((x, y) => String.Compare(x.entityName, y.entityName, StringComparison.Ordinal));
        foreach (var (_, lockAsync) in locks)
        {
            ct.ThrowIfCancellationRequested();
            await lockAsync(context, ct);
        }
    }
}