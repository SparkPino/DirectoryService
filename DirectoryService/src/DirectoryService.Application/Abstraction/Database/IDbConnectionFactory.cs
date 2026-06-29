using System.Data;

namespace DirectoryService.Application.Abstraction.Database;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}