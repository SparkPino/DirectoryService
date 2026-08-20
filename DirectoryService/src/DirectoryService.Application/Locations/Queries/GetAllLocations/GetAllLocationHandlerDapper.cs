using System.Text;
using System.Text.Json;
using Core;
using Core.Database;
using Core.Validations;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Contracts.Locations;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Locations.Queries.GetAllLocations;

public class GetAllLocationHandlerDapper(
    IDbConnectionFactory connection,
    ILogger<GetAllLocationHandlerDapper> logger,
    IValidator<GetAllLocationQueryDapper> validator)
    : IQueryHandler<GetAllLocationQueryDapper, PagedResult<GetAllLocationDto>>
{
    private readonly IDbConnectionFactory _connection = connection;
    private readonly ILogger<GetAllLocationHandlerDapper> _logger = logger;
    private readonly IValidator<GetAllLocationQueryDapper> _validator = validator;

    private static readonly JsonSerializerOptions _addressJsonOptions =
        new() { PropertyNameCaseInsensitive = true, };

    public async Task<Result<PagedResult<GetAllLocationDto>, Errors>> Handle(
        GetAllLocationQueryDapper query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid) return validationResult.ToError();

        _logger.LogInformation("Start handle GetAllLocationDapper");

        StringBuilder sb = new StringBuilder();
        var parameters = new DynamicParameters();

        sb.AppendLine(@"
        SELECT 
       l.id                    AS ""Id"",
       l.name                  AS ""Name"",
       l.addresses             AS ""Address"",
       l.created_at            AS ""CreatedAt"",
       COUNT(dl.department_id) AS ""AttachDepartmentCount"",
       COUNT(*) OVER()         AS ""TotalCount""
        FROM locations l
        LEFT JOIN departments_location dl ON l.id = dl.location_id
        WHERE is_active = TRUE");


        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            sb.AppendLine(" AND name ILIKE @Search ");
            parameters.Add("Search", $"%{query.Search}%");
        }

        sb.Append(" GROUP BY l.id, l.name, l.addresses, l.created_at");

        if (query.MinDepartmentCount.HasValue && query.MinDepartmentCount > 0)
        {
            sb.AppendLine(" HAVING  COUNT(dl.department_id)  >= @MinDepartmentCount ");
            parameters.Add("MinDepartmentCount", query.MinDepartmentCount.Value);
        }


        string defaultOrderBy = "l.name";
        string defaultDirection = " DESC";
        bool descending;
        if (!string.IsNullOrWhiteSpace(query.OrderBy))
        {
            defaultOrderBy = query.OrderBy switch
            {
                "name" => "l.name",
                "createdat" => "l.created_at",
                "departmentcount" => "DepartmentCount",
                _ => defaultOrderBy,
            };
        }

        if (query.SortDirection.HasValue)
        {
            descending = query.SortDirection == SortDirection.DESC;
            defaultDirection = descending ? " DESC" : " ASC";
        }

        sb.AppendLine($" ORDER BY {defaultOrderBy} {defaultDirection}, l.id");

        int defaultPageSize = 20;
        int defaultPage = 1;

        int page = query.Page ?? defaultPage;
        int pageSize = query.PageSize ?? defaultPageSize;

        sb.Append($" LIMIT {pageSize} OFFSET {(page - 1) * pageSize}");

        using var connectionCreated = await _connection.CreateConnectionAsync(cancellationToken);

        var rowResult =
            await connectionCreated.QueryAsync<GetAllLocationRow>(new CommandDefinition(sb.ToString(), parameters,
                cancellationToken: cancellationToken));

        var result = rowResult.Select(r => new GetAllLocationDto()
        {
            Name = r.Name,
            Address = JsonSerializer.Deserialize<AddressDto>(r.Address, _addressJsonOptions)!,
            CreatedAt = r.CreatedAt,
            AttachDepartmentCount = r.AttachDepartmentCount,
        }).ToList();

        var count = rowResult.Select(r => r.TotalCount).FirstOrDefault();
        return new PagedResult<GetAllLocationDto>(result.ToList(), count);
    }
}