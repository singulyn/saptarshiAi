using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SaptariX.Persistence.Abstractions;

namespace SaptariX.Persistence.SqlServer;

public sealed class SqlServerConnectionFactory : IDbConnectionFactory
{
    private readonly IConfiguration _configuration;
    private readonly SqlServerOptions _options;

    public SqlServerConnectionFactory(IConfiguration configuration, IOptions<SqlServerOptions> options)
    {
        _configuration = configuration;
        _options = options.Value;
    }

    public async Task<IDbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connectionString = _configuration.GetConnectionString(_options.ConnectionStringName);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException($"Missing SQL Server connection string '{_options.ConnectionStringName}'.");
        }

        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
