namespace SaptariX.Persistence.Abstractions;

public interface IStoredProcedureExecutor
{
    Task<IReadOnlyList<T>> QueryAsync<T>(
        string storedProcedure,
        object? parameters = null,
        int? timeoutSeconds = null,
        CancellationToken cancellationToken = default);

    Task<T?> QuerySingleOrDefaultAsync<T>(
        string storedProcedure,
        object? parameters = null,
        int? timeoutSeconds = null,
        CancellationToken cancellationToken = default);

    Task<int> ExecuteAsync(
        string storedProcedure,
        object? parameters = null,
        int? timeoutSeconds = null,
        CancellationToken cancellationToken = default);
}
