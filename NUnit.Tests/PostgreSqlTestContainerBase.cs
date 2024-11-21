using Testcontainers.PostgreSql;
using Tests;

namespace NUnit.Tests;

public abstract class PostgreSqlTestContainerBase
{
	private readonly PostgreSqlContainer postgreSqlContainer =
		new PostgreSqlBuilder()
			.WithImage("postgres:latest")
			.Build();
	
	[OneTimeSetUp]
	public async Task OneTimeSetUpAsync()
	{
		await postgreSqlContainer.StartAsync();
		await postgreSqlContainer.WaitForPort();
	}
	
	[OneTimeTearDown]
	public async Task OneTimeTearDownAsync()
	{
		await postgreSqlContainer.StopAsync();
		await postgreSqlContainer.DisposeAsync();
	}

	private protected String ConnectionString => postgreSqlContainer.GetConnectionString();
}