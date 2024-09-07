using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using EFCore.PostgresExtensions.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

{
    await using var context = new Context();
    await context.Database.EnsureDeletedAsync();
    await context.Database.EnsureCreatedAsync();
}

// await using (var context = new Context())
// {
//     context.Set<Entity>().AddRange(new Entity(), new Entity(), new Entity());
//     await context.SaveChangesAsync();
//     var tx = await context.Database.BeginTransactionAsync();
//     await context.Set<Entity>().LockRowsAsync(x => true);
//     await tx.CommitAsync();
// }

// Int64 id1;
// await using (var context = new Context())
// {
//     var entity = new Entity();
//     context.Add(entity);
//     await context.SaveChangesAsync();
//     id1 = entity.Id;
// }
//
// await IncrementCountAsAnUpdateAsync(id1);

//  
// await using (var context = new Context())
// {
//     var entity = await context.Set<Entity>().SingleAsync(x => x.Id == id1);
//     Console.WriteLine(entity.Id + " " + entity.Count);
// }
 
// await using (var context = new Context())
// {
//     await context.WithTransactionAsync(
//         x => x.AddLock<Entity>(e => e.Id == id1),
//         async (ctx, ct) =>
//         {
//             var entity = await ctx.Set<Entity>().SingleAsync(x => x.Id == id1, ct);
//             entity.Count++;
//             await ctx.SaveChangesAsync(ct);
//         }
//     );
// }


Func<Int64, Task>[] funcs = [
    IncrementCountAsAnUpdateAsync,
    IncrementCountAsSelectThenUpdateUnsafeAsync,
    IncrementCountAsSelectThenUpdateSafeAsync,
    IncrementCountAsSelectThenUpdate_WithTransactionAsync,
    // IncrementCountAsSelectThenUpdate_SingleLockedAsync,
    IncrementCountAsSelectThenUpdate_SingleRowLockedUpdateAsync,
    //IncrementCountAsSelectThenUpdateInSerializableIsolationAsync,
];
foreach (var func in funcs)
{
    Int64 id;
    await using (var context = new Context())
    {
        var entity = new Entity();
        context.Add(entity);
        await context.SaveChangesAsync();
        id = entity.Id;
    }

    Stopwatch sw = Stopwatch.StartNew();
    var a = Enumerable.Range(0, 100).Select(_ => func(id));
    await Task.WhenAll(a);
    sw.Stop();
    Console.Write($"{func.Method.Name}: {sw.ElapsedMilliseconds}: ");
    
    await using (var context = new Context())
    {
        var entity = await context.Set<Entity>().SingleAsync(x => x.Id == id);
        Console.WriteLine(entity.Count);
    }
}

Console.WriteLine(ForUpdateDbCommandInterceptor.InterceptedCount);
Console.WriteLine(ForUpdateDbCommandInterceptor.InstanceRetrievedCount);
return;

{
    await using var context = new Context();
    var entity = await context.Set<Entity>().SingleAsync();
    Console.WriteLine(entity.Count);
}

return;

static async Task IncrementCountAsAnUpdateAsync(Int64 id)
{
    await using var context = new Context();
    await context.Set<Entity>().Where(x => x.Id == id).ExecuteUpdateAsync(x => x.SetProperty(e => e.Count, e => e.Count + 1));
}

static async Task IncrementCountAsSelectThenUpdateUnsafeAsync(Int64 id)
{
    await using var context = new Context();
    var entity = await context.Set<Entity>().SingleAsync(x => x.Id == id);
    entity.Count++;
    await context.SaveChangesAsync();
}

static async Task IncrementCountAsSelectThenUpdateSafeAsync(Int64 id)
{
    await using var context = new Context();
    await using var tx = await context.Database.BeginTransactionAsync();
    Expression<Func<Entity, Boolean>> predicate = x => x.Id == id;
    await context.Set<Entity>().LockRowsAsync(tx, predicate);
    var entity = await context.Set<Entity>().SingleAsync(predicate);
    entity.Count++;
    await context.SaveChangesAsync();
    await tx.CommitAsync();
}

static async Task IncrementCountAsSelectThenUpdate_WithTransactionAsync(Int64 id)
{
    await using var context = new Context();
    await context.WithTransactionAsync(
        x => x.AddLock<Entity>(e => e.Id == id),
        async (ctx, ct) =>
        {
            var entity = await ctx.Set<Entity>().SingleAsync(x => x.Id == id, ct);
            entity.Count++;
            await ctx.SaveChangesAsync(ct);
        }
    );
}

static async Task IncrementCountAsSelectThenUpdate_SingleRowLockedUpdateAsync(Int64 id)
{
    await using var context = new Context();
    await context.SingleRowLockedUpdateAsync<Entity>(x => x.Id == id, x => x.Count++);
}

// static async Task IncrementCountAsSelectThenUpdateInSerializableIsolationAsync(Int64 id)
// {
//     await using var context = new Context();
//     var tx = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
//     Expression<Func<Entity, Boolean>> predicate = x => x.Id == id;
//     var entity = await context.Set<Entity>().SingleAsync(predicate);
//     entity.Count++;
//     await context.SaveChangesAsync();
//     await tx.CommitAsync();
// }

public class Context(String? connectionString = null) : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder x) => x
        .UseNpgsql(connectionString ?? "Host=localhost;Database=nick;Include Error Detail=true;User ID=nick;Password=nick;")
        .AddInterceptors(ForUpdateDbCommandInterceptor.Instance)
        //.LogTo(Console.WriteLine, LogLevel.Trace)
        ;

    protected override void ConfigureConventions(ModelConfigurationBuilder x) =>
        x.Properties<String>().HaveMaxLength(1000);

    protected override void OnModelCreating(ModelBuilder mb) =>
        mb.Entity<Entity>();
}

public class Entity
{
    public Int64 Id { get; set; }
    public Int64 Count { get; set; }
}

public sealed class Box<T> where T : struct
{
    public T Value { get; set; }
}