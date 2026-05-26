using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Departments.Failures;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
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

    public Task<Result<Guid, Error>> SaveAsync(Department department, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public async Task<Result<Unit, Error>> DeleteByIdAsync(Guid departmentId, CancellationToken cancellationToken)
    {
        var correctId = new DepartmentId(departmentId);
        var department = await _context.Departments.FindAsync([correctId], cancellationToken);
        if (department == null)
        {
            _logger.LogWarning("Департамент с id:{departmentId} не найден при удалении", departmentId);
            return DepartmentError.NotFound(departmentId);
        }

        _context.Departments.Remove(department);
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
        {
            var errorDescription = DepartmentError.Database;
            _logger.LogError(exception.InnerException ?? exception,
                "Ошибка базы данных при удалении департамента {DepartmentId}", departmentId);
            return errorDescription;
        }

        _logger.LogInformation("Департамент {DepartmentId} успешно удалён", departmentId);
        return Unit.Value;
    }

    public async Task<Result<Department, Error>> GetByIdAsync(Guid departmentId, CancellationToken cancellationToken)
    {
        var correctLocationId = new DepartmentId(departmentId);

        var department = await _context.Departments
            .Where(l => l.Id == correctLocationId)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (department == null)
        {
            _logger.LogWarning("Департамент не найден Id:{departmentId}", departmentId);
            return DepartmentError.NotFound(departmentId);
        }

        return department;
    }
}