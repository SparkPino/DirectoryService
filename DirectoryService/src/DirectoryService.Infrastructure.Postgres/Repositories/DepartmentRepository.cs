using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Departments.Failures;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
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

    public async Task<Result<Guid, Error>> AddAsync(Department department, CancellationToken cancellationToken)
    {
        await _context.Departments.AddAsync(department, cancellationToken);
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return department.Id.Id;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex.InnerException ?? ex,
                "Конфликт параллельного доступа при добавлении департамента {DepartmentId}", department.Id);
            return DepartmentError.Concurrency;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex.InnerException ?? ex, "Ошибка базы данных при добавлении департамента {LocationId}",
                department.Id);
            return DepartmentError.Database;
        }
    }

    public async Task<int> SaveAsync(CancellationToken cancellationToken) =>
        await _context.SaveChangesAsync(cancellationToken);

    public async Task<Result<Unit, Error>> DeleteByIdAsync(Guid departmentId, CancellationToken cancellationToken)
    {
        int deletedCount;
        try
        {
            deletedCount = await _context.Departments
                .Where(d => d.Id.Id == departmentId)
                .ExecuteDeleteAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(
                exception.InnerException ?? exception,
                "Ошибка базы данных при удалении департамента {DepartmentId}", departmentId);
            return DepartmentError.Database;
        }

        if (deletedCount == 0)
        {
            _logger.LogWarning("Департамент с Id:{DepartmentId} не найден при удалении", departmentId);
            return DepartmentError.NotFound(departmentId);
        }

        _logger.LogInformation("Департамент {DepartmentId} успешно удалён", departmentId);
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
}