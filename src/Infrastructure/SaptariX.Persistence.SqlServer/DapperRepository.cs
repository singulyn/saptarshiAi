using System.Data;
using Dapper;
using Microsoft.Extensions.Options;
using SaptariX.Persistence.Abstractions;

namespace SaptariX.Persistence.SqlServer;

public sealed class DapperRepository : IDapperRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly SqlServerOptions _options;

    public DapperRepository(IDbConnectionFactory connectionFactory, IOptions<SqlServerOptions> options)
    {
        _connectionFactory = connectionFactory;
        _options = options.Value;
    }

    public async Task<IReadOnlyList<T>> QueryStoredProcedureAsync<T>(
        string storedProcedure,
        object? parameters = null,
        int? timeoutSeconds = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = BuildCommand(storedProcedure, parameters, timeoutSeconds, cancellationToken);
        var rows = await connection.QueryAsync<T>(command);
        return rows.AsList();
    }

    public async Task<T?> QuerySingleOrDefaultStoredProcedureAsync<T>(
        string storedProcedure,
        object? parameters = null,
        int? timeoutSeconds = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = BuildCommand(storedProcedure, parameters, timeoutSeconds, cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<T>(command);
    }

    public async Task<int> ExecuteStoredProcedureAsync(
        string storedProcedure,
        object? parameters = null,
        int? timeoutSeconds = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = BuildCommand(storedProcedure, parameters, timeoutSeconds, cancellationToken);
        return await connection.ExecuteAsync(command);
    }

    private CommandDefinition BuildCommand(
        string storedProcedure,
        object? parameters,
        int? timeoutSeconds,
        CancellationToken cancellationToken)
    {
        return new CommandDefinition(
            storedProcedure,
            parameters,
            commandTimeout: timeoutSeconds ?? _options.DefaultCommandTimeoutSeconds,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);
    }
}
