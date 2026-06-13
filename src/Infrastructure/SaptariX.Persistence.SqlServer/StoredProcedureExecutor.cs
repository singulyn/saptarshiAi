using SaptariX.Persistence.Abstractions;

namespace SaptariX.Persistence.SqlServer;

public sealed class StoredProcedureExecutor : IStoredProcedureExecutor
{
    private readonly IDapperRepository _repository;

    public StoredProcedureExecutor(IDapperRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<T>> QueryAsync<T>(
        string storedProcedure,
        object? parameters = null,
        int? timeoutSeconds = null,
        CancellationToken cancellationToken = default)
    {
        return _repository.QueryStoredProcedureAsync<T>(storedProcedure, parameters, timeoutSeconds, cancellationToken);
    }

    public Task<T?> QuerySingleOrDefaultAsync<T>(
        string storedProcedure,
        object? parameters = null,
        int? timeoutSeconds = null,
        CancellationToken cancellationToken = default)
    {
        return _repository.QuerySingleOrDefaultStoredProcedureAsync<T>(storedProcedure, parameters, timeoutSeconds, cancellationToken);
    }

    public Task<int> ExecuteAsync(
        string storedProcedure,
        object? parameters = null,
        int? timeoutSeconds = null,
        CancellationToken cancellationToken = default)
    {
        return _repository.ExecuteStoredProcedureAsync(storedProcedure, parameters, timeoutSeconds, cancellationToken);
    }
}
