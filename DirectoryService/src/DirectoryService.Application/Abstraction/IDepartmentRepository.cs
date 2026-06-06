using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using Shared;

namespace DirectoryService.Application.Abstraction;

public interface IDepartmentRepository
{
    Task<Result<Guid, Error>> AddAsync(Department department, CancellationToken cancellationToken);

    Task<int> SaveAsync(CancellationToken cancellationToken);

    Task<Result<Unit, Error>> DeleteByIdAsync(Guid departmentId, CancellationToken cancellationToken);

    Task<Result<Department, Error>> GetByIdAsync(Guid departmentId, CancellationToken cancellationToken);

    Task<bool> IsLocationAttachedAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken);
    Task<Result<Department, Error>> GetByIdWithLocationsAsync(Guid departmentId, CancellationToken cancellationToken);
}