using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Abstraction.Database;
using DirectoryService.Application.Departments.Failures;
using DirectoryService.Contracts.Department;
using DirectoryService.Domain.Departments.ValueObjects;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Departments.Queries.GetByIdDepartment;

public class GetByIdDepartmentHandler(
    IReadDbContext context,
    IValidator<GetByIdDepartmentQuery> validator,
    ILogger<GetByIdDepartmentHandler> logger)
    : IQueryHandler<GetByIdDepartmentQuery, DepartmentDto>
{
    private readonly IReadDbContext _context = context;
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

        var department = await _context.ReadDepartments
            .Where(a => a.Id == departmentId)
            .Select(a => new DepartmentDto(
                a.Name.Value,
                a.Identifier.Identifier,
                a.DepartmentsLocations.Select(dl => dl.LocationId.Id),
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