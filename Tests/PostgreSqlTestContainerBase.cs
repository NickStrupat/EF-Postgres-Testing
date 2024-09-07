using Testcontainers.PostgreSql;

namespace Tests;

public abstract class PostgreSqlTestContainerBase : IAsyncLifetime
{
    private readonly PostgreSqlContainer postgreSqlContainer =
        new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .Build();

    async Task IAsyncLifetime.InitializeAsync()
    {
        await postgreSqlContainer.StartAsync();
        await postgreSqlContainer.WaitForPort();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await postgreSqlContainer.StopAsync();
        await postgreSqlContainer.DisposeAsync();
    }

    private protected String ConnectionString => postgreSqlContainer.GetConnectionString();
}