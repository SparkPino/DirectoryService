using System.Text.Json;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Abstraction.Database;
using DirectoryService.Contracts.Locations;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Locations.Queries.GetLocationTop;

public class GetLocationTopHandler : IQueryHandler<IReadOnlyCollection<GetLocationTopDto>>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly ILogger<GetLocationTopHandler> _logger;

    public GetLocationTopHandler(
        IDbConnectionFactory dbConnectionFactory,
        ILogger<GetLocationTopHandler> logger)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyCollection<GetLocationTopDto>, Errors>> Handle(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("обоработка GetLocationTopDto начата");
        var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var rows = await connection.QueryAsync<GetLocationTopRow>(
            """
            SELECT
                loc.name,
                loc.addresses,
                COUNT(dl.department_id) AS department_count
            FROM departments_location dl
            INNER JOIN locations loc ON loc.id = dl.location_id
            INNER JOIN departments d ON d.id = dl.department_id
            WHERE loc.is_active = true AND d.is_active = true
            GROUP BY loc.id
            ORDER BY department_count DESC, loc.id
            LIMIT 5;
            """);

        var result = rows.Select(s => new GetLocationTopDto()
        {
            Name = s.Name,
            Addresses = JsonSerializer.Deserialize<LocationAdressDto>(s.Addresses),
            departmentCount = s.departmentCount,
        }).ToList();

        return result;
    }
}