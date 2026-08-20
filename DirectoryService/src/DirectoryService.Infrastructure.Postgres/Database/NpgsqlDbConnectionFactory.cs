using System.Data;
using Core.Database;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace DirectoryService.Infrastructure.Postgres.Database;

public sealed class NpgsqlDbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public NpgsqlDbConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString(DbConstants.DIRECTORY_SERVICE_DB_CONNECTION_STRING_KEY)
            ?? throw new InvalidOperationException("Connection string not found.");
    }

    public NpgsqlDbConnectionFactory(string connectionString) // for tests
    {
        _connectionString = connectionString;
    }

    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}