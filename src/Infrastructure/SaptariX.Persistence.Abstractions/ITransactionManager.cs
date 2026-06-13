using System.Data;

namespace SaptariX.Persistence.Abstractions;

public interface ITransactionManager
{
    Task ExecuteAsync(
        Func<IDbConnection, IDbTransaction, Task> action,
        CancellationToken cancellationToken = default);

    Task<T> ExecuteAsync<T>(
        Func<IDbConnection, IDbTransaction, Task<T>> action,
        CancellationToken cancellationToken = default);
}
