using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;

public static class NpgsqlExtensions
{
    // public static async Task LockRowsAsync<T>(this DbContext context, Expression<Func<T, Boolean>> predicate, CancellationToken ct = default) where T : class
    // {
    //     ForUpdateDbCommandInterceptor.ShouldAppendForUpdate.Value = true;
    //     await context.Set<T>().Where(predicate).Select(_ => 0).ToListAsync(ct);
    //     // var query = context.Set<T>().Where(predicate).Select(_ => 0).ToQueryString();
    //     // var forNoKeyUpdate = query + " for update";
    //     // Console.WriteLine(forNoKeyUpdate);
    //     // //await context.Database.SqlQueryRaw<T>(forNoKeyUpdate, ct);
    //     // _  = await context.Database.SqlQueryRaw<T>(forNoKeyUpdate).Take(1).SingleOrDefaultAsync(ct);
    // }

    public static async Task LockRowsAsync<T>(this DbSet<T> dbSet, IDbContextTransaction transaction, Expression<Func<T, Boolean>> predicate, CancellationToken ct = new()) where T : class
    {
        ArgumentNullException.ThrowIfNull(dbSet);
        ArgumentNullException.ThrowIfNull(transaction);
        ArgumentNullException.ThrowIfNull(predicate);
        using (new ForUpdateDbCommandInterceptor.ForUpdateCommandModificationScope())
            _ = await dbSet.IgnoreAutoIncludes().AsNoTracking().Where(predicate).Select(_ => 0).ToListAsync(ct);
    }
    
    public static async Task WithTransactionAsync(
        this DbContext context,
        Action<LockBuilder> lockBuilderAction,
        Func<DbContext, CancellationToken, Task> whenLockedAsync,
        CancellationToken ct = new())
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(lockBuilderAction);
        ArgumentNullException.ThrowIfNull(whenLockedAsync);
        await using var tx = await context.Database.BeginTransactionAsync(ct);
        try
        {
            LockBuilder lb = new();
            lockBuilderAction(lb);
            using (new ForUpdateDbCommandInterceptor.ForUpdateCommandModificationScope())
                await lb.ApplyLocksAsync(context, ct);
            await whenLockedAsync(context, ct);
            await tx.CommitAsync(ct);
        }
        catch (Exception)
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }
    
    public static async Task SingleRowLockedUpdateAsync<T>(
        this DbContext context,
        Expression<Func<T, Boolean>> predicate,
        Action<T> update,
        CancellationToken ct = new()
    )
        where T : class
    {
        ArgCheck.ThrowIfNull(context, predicate, update);
        
        await context.SingleRowLockedUpdateAsync(predicate, (entity, ct) =>
        {
            ct.ThrowIfCancellationRequested();
            update(entity);
            return Task.CompletedTask;
        }, ct);
    }
    
    public static async Task SingleRowLockedUpdateAsync<T>(
        this DbContext context,
        Expression<Func<T, Boolean>> predicate,
        Func<T, CancellationToken, Task> update,
        CancellationToken ct = new()
    )
    where T : class
    {
        ArgCheck.ThrowIfNull(context, predicate, update);
        
        await using var tx = await context.Database.BeginTransactionAsync(ct);
        try
        {
            T single;
            using (new ForUpdateDbCommandInterceptor.ForUpdateCommandModificationScope())
                single = await context.Set<T>().SingleAsync(predicate, ct);
            await update(single, ct);
            await context.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }
        catch (Exception)
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }
    
    public static async Task SingleRowLockedUpdateAsync<T>(
        this IDbContextFactory<DbContext> dbContextFactory,
        Expression<Func<T, Boolean>> predicate,
        Func<T, CancellationToken, Task> update,
        CancellationToken ct = new()
    )
        where T : class
    {
        ArgumentNullException.ThrowIfNull(dbContextFactory);
        
        await using var context = await dbContextFactory.CreateDbContextAsync(ct);
        await context.SingleRowLockedUpdateAsync(predicate, update, ct);
    }
}