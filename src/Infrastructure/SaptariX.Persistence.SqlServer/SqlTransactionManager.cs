using System.Data;
using SaptariX.Persistence.Abstractions;

namespace SaptariX.Persistence.SqlServer;

public sealed class SqlTransactionManager : ITransactionManager
{
    private readonly IDbConnectionFactory _connectionFactory;

    public SqlTransactionManager(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task ExecuteAsync(
        Func<IDbConnection, IDbTransaction, Task> action,
        CancellationToken cancellationToken = default)
    {
        await ExecuteAsync(async (connection, transaction) =>
        {
            await action(connection, transaction);
            return true;
        }, cancellationToken);
    }

    public async Task<T> ExecuteAsync<T>(
        Func<IDbConnection, IDbTransaction, Task<T>> action,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        try
        {
            var result = await action(connection, transaction);
            transaction.Commit();
            return result;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
