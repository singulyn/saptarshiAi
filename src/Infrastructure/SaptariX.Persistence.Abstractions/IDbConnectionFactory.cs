using System.Data;

namespace SaptariX.Persistence.Abstractions;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default);
}
