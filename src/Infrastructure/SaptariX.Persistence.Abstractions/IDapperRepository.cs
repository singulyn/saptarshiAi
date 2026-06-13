namespace SaptariX.Persistence.Abstractions;

public interface IDapperRepository
{
    Task<IReadOnlyList<T>> QueryStoredProcedureAsync<T>(
        string storedProcedure,
        object? parameters = null,
        int? timeoutSeconds = null,
        CancellationToken cancellationToken = default);

    Task<T?> QuerySingleOrDefaultStoredProcedureAsync<T>(
        string storedProcedure,
        object? parameters = null,
        int? timeoutSeconds = null,
        CancellationToken cancellationToken = default);

    Task<int> ExecuteStoredProcedureAsync(
        string storedProcedure,
        object? parameters = null,
        int? timeoutSeconds = null,
        CancellationToken cancellationToken = default);
}
