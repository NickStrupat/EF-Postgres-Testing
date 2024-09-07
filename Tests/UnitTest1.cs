using Microsoft.EntityFrameworkCore;

namespace Tests;

public class UnitTest1 : PostgreSqlTestContainerBase
{
    [Fact]
    public async Task Test1()
    {
        await using var context = new Context(ConnectionString);
        await context.Database.EnsureCreatedAsync();
        context.Set<Entity>().Add(new Entity { Count = 42 });
        await context.SaveChangesAsync();
        var entity = await context.Set<Entity>().SingleAsync();
        Assert.Equal(42, entity.Count);
    }
}