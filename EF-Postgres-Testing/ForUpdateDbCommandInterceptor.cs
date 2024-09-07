using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;

internal sealed class ForUpdateDbCommandInterceptor : DbCommandInterceptor
{
    private ForUpdateDbCommandInterceptor() {}

    private static readonly ForUpdateDbCommandInterceptor instance = new();
    public static ForUpdateDbCommandInterceptor Instance
    {
        get
        {
            InstanceRetrievedCount.Increment();
            return instance;
        }
    }
    
    public async Task EnableWhile(Func<Task> action)
    {
        using var _ = new ForUpdateCommandModificationScope();
        await action();
    }

    public struct ForUpdateCommandModificationScope : IDisposable
    {
        public ForUpdateCommandModificationScope() => ShouldAppendForUpdate.Value = true;
        public void Dispose() => ShouldAppendForUpdate.Value = false;
    }
    
    private static readonly AsyncLocal<Boolean> ShouldAppendForUpdate = new();
    internal static InterlockedInt64 InterceptedCount;
    internal static InterlockedInt64 InstanceRetrievedCount;

    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        ModifyCommand(command);
        return base.ReaderExecuting(command, eventData, result);
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken ct = new())
    {
        ModifyCommand(command);
        return base.ReaderExecutingAsync(command, eventData, result, ct);
    }

    private static void ModifyCommand(DbCommand command)
    {
        var connection = command.Connection ?? throw new InvalidOperationException("The command has no connection.");
        if (connection is not NpgsqlConnection)
            throw new InvalidOperationException($"This interceptor only supports Npgsql connections. The connection in use is of type {connection.GetType()}.");
        
        if (ShouldAppendForUpdate.Value && command.CommandText.StartsWith("select", StringComparison.OrdinalIgnoreCase))
        {
            command.CommandText += " for no key update";
            InterceptedCount.Increment();
        }
    }
}