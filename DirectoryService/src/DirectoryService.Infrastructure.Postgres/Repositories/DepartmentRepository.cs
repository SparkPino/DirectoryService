using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Departments.Failures;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Positions.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly DirectoryServiceDbContext _context;
    private readonly ILogger<DepartmentRepository> _logger;

    public DepartmentRepository(DirectoryServiceDbContext context, ILogger<DepartmentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Guid> AddAsync(Department department, CancellationToken cancellationToken)
    {
        await _context.Departments.AddAsync(department, cancellationToken);
        return department.Id.Id;
    }
    

    public async Task<Result<Unit, Error>> DeleteByIdAsync(Guid departmentId, CancellationToken cancellationToken)
    {
        var correctId = new DepartmentId(departmentId);

        var department = await _context.Departments
            .FirstOrDefaultAsync(d => d.Id == correctId, cancellationToken);

        if (department == null)
        {
            _logger.LogWarning("Департамент с Id:{DepartmentId} не найден при удалении", departmentId);
            return DepartmentError.NotFound(departmentId);
        }

        _context.Departments.Remove(department);

        return Unit.Value;
    }

    public async Task<Result<Department, Error>> GetByIdAsync(Guid departmentId, CancellationToken cancellationToken)
    {
        var correctLocationId = new DepartmentId(departmentId);

        var department = await _context.Departments
            .FirstOrDefaultAsync(l => l.Id == correctLocationId, cancellationToken);
        if (department == null)
        {
            _logger.LogWarning("Департамент не найден Id:{departmentId}", departmentId);
            return DepartmentError.NotFound(departmentId);
        }

        return department;
    }

    public async Task<bool> IsLocationAttachedAsync(Guid departmentId, Guid locationId,
        CancellationToken cancellationToken)
    {
        bool result =
            await _context.DepartmentLocations.AnyAsync(
                a => a.LocationId == new LocationId(locationId) && a.DepartmentId == new DepartmentId(departmentId),
                cancellationToken);
        return result;
    }

    public async Task<Result<Department, Error>> GetByIdWithLocationsAsync(
        Guid departmentId,
        CancellationToken cancellationToken)
    {
        var correctId = new DepartmentId(departmentId);

        var department = await _context.Departments
            .Include(d => d.DepartmentsLocations)
            .FirstOrDefaultAsync(d => d.Id == correctId, cancellationToken);

        if (department == null)
        {
            _logger.LogWarning("Департамент не найден Id:{departmentId}", departmentId);
            return DepartmentError.NotFound(departmentId);
        }

        return department;
    }

    public async Task<bool> IsPositionAttachedAsync(Guid departmentId, Guid positionId,
        CancellationToken cancellationToken)
    {
        bool result =
            await _context.DepartmentPositions.AnyAsync(
                a => a.PositionId == new PositionId(positionId) && a.DepartmentId == new DepartmentId(departmentId),
                cancellationToken);
        return result;
    }

    public async Task<Result<Department, Error>> GetByIdWithPositionsAsync(
        Guid departmentId,
        CancellationToken cancellationToken)
    {
        var correctId = new DepartmentId(departmentId);

        var department = await _context.Departments
            .Include(d => d.Positions)
            .FirstOrDefaultAsync(d => d.Id == correctId, cancellationToken);

        if (department == null)
        {
            _logger.LogWarning("Департамент не найден Id:{departmentId}", departmentId);
            return DepartmentError.NotFound(departmentId);
        }

        return department;
    }

    public async Task<int> UpdateDescendantsPathAsync(
        DepartmentPath oldPath,
        DepartmentPath newPath,
        short depthDelta,
        CancellationToken cancellationToken)
    {
        string oldPrefix = $"{oldPath.Path}.";
        string newPrefix = $"{newPath.Path}.";

        return await _context.Departments
            .Where(d => d.Path.Path.StartsWith(oldPrefix))
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(d => d.Path.Path, d => newPrefix + d.Path.Path.Substring(oldPrefix.Length))
                    .SetProperty(d => d.Depth, d => (short)(d.Depth + depthDelta))
                    .SetProperty(d => d.UpdatedAt, _ => DateTimeOffset.UtcNow),
                cancellationToken);
    }
}