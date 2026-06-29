using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Abstraction.Database;
using DirectoryService.Application.Departments.Failures;
using DirectoryService.Contracts.Department;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Departments.ValueObjects;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Departments.Queries.GetByIdDepartment;

public class GetByIdDapper(
    IReadDbContext context,
    IDbConnectionFactory dbConnectionFactory,
    IValidator<GetByIdDepartmentQuery> validator,
    ILogger<GetByIdDepartmentHandler> logger)
    : IQueryHandler<GetByIdDepartmentQuery, DepartmentRow>
{
    private readonly IReadDbContext _context = context;
    private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;
    private readonly IValidator<GetByIdDepartmentQuery> _validator = validator;
    private readonly ILogger<GetByIdDepartmentHandler> _logger = logger;

    public async Task<Result<DepartmentRow, Errors>> Handle(
        GetByIdDepartmentQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
        {
            return DepartmentError.InvalidId.ToErrors();
        }


        DepartmentRow departmentRow = null;
        var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        var department = await connection.QueryAsync<DepartmentRow, ShortAdressDTO, DepartmentRow>(
            """
            SELECT
            d.name,
            d.identifier,
            d.xmin::text::bigint AS row_version,
            d.parent_id,
                
            l.id
            FROM departments d
            JOIN departments_location dl ON d.id=dl.department_id     
            LEFT JOIN locations l ON l.id = dl.location_id
            WHERE d.id = @queryId;
            """,
            param: new
            {
                queryId = query.id // @queryId = queryId
            },
            splitOn: "id",
            map:
            (DepartmentRow, LocationResponseDto) =>
            {
                if (departmentRow is null)
                {
                    departmentRow = DepartmentRow;
                }

                departmentRow.LocationIds.Add(LocationResponseDto.Id);
                return departmentRow;
            });


        if (department is null)
        {
            _logger.LogWarning("Департамент с id:{DepartmentId} не найден.", query.id);
            return DepartmentError.NotFound(query.id).ToErrors();
        }

        return department.FirstOrDefault()!;
    }
}