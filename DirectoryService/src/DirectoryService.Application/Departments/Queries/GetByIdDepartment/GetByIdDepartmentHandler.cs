using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Abstraction.Database;
using DirectoryService.Application.Departments.Failures;
using DirectoryService.Contracts.Department;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Departments.Queries.GetByIdDepartment;

public class GetByIdDepartmentHandler(
    IReadDbContext context,
    IDbConnectionFactory dbConnectionFactory,
    IValidator<GetByIdDepartmentQuery> validator,
    ILogger<GetByIdDepartmentHandler> logger)
    : IQueryHandler<GetByIdDepartmentQuery, DepartmentDto>
{
    private readonly IReadDbContext _context = context;
    private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;
    private readonly IValidator<GetByIdDepartmentQuery> _validator = validator;
    private readonly ILogger<GetByIdDepartmentHandler> _logger = logger;

    public async Task<Result<DepartmentDto, Errors>> Handle(
        GetByIdDepartmentQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
        {
            return DepartmentError.InvalidId.ToErrors();
        }

        DepartmentId departmentId = new DepartmentId(query.id);

        // var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        /*var result = await connection.QueryAsync<DepartmentRow>(
            """
            SELECT
            d.name,
            d.identifier,
            array_agg(dl.location_id) AS location_ids,
            d.xmin::text::bigint AS row_version,
            d.parent_id
            FROM departments d
            LEFT JOIN departments_location dl ON d.id = dl.department_id
            WHERE d.id = @queryId
            GROUP BY d.id;
            """,
            param: new { queryId = query.id });*/ // @queryId = queryId


        var department = await _context.ReadDepartments
            .Where(a => a.Id == departmentId)
            .Select(a => new DepartmentDto(
                a.Name.Value,
                a.Identifier.Identifier,
                a.DepartmentsLocations
                    .Where(dl => _context.ReadLocations
                        .Select(l => l.Id)
                        .Contains(dl.LocationId))
                    .Select(dl => dl.LocationId.Id).ToList(), // сам делает джоин
                a.RowVersion,
                a.ParentId != null ? a.ParentId.Id : (Guid?)null))
            .FirstOrDefaultAsync(cancellationToken);

        if (department is null)
        {
            _logger.LogWarning("Департамент с id:{DepartmentId} не найден.", query.id);
            return DepartmentError.NotFound(query.id).ToErrors();
        }

        return department;
    }
}