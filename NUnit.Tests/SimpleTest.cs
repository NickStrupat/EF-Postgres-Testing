using Microsoft.EntityFrameworkCore;

namespace NUnit.Tests;

public class SimpleTest : PostgreSqlTestContainerBase 
{
	[Test]
	public async Task Test1()
	{
		await using var context = new Context(ConnectionString);
		await context.Database.EnsureCreatedAsync();
		context.Set<Entity>().Add(new Entity { Count = 42 });
		await context.SaveChangesAsync();
		var entity = await context.Set<Entity>().SingleAsync();
		Assert.That(42 == entity.Count);
	}
}