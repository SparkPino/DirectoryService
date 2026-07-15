using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Positions;
using Shared;

namespace DirectoryService.Application.Abstraction.Repositories;

public interface IDepartmentRepository
{
    Task<Guid> AddAsync(Department department, CancellationToken cancellationToken);

    Task<Result<Unit, Error>> DeleteByIdAsync(Guid departmentId, CancellationToken cancellationToken);

    Task<Result<Department, Error>> GetByIdAsync(Guid departmentId, CancellationToken cancellationToken);

    Task<bool> IsLocationAttachedAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken);

    Task<Result<Department, Error>> GetByIdWithLocationsAsync(Guid departmentId, CancellationToken cancellationToken);

    Task<bool> IsPositionAttachedAsync(Guid departmentId, Guid positionId, CancellationToken cancellationToken);

    Task<Result<Department, Error>> GetByIdWithPositionsAsync(Guid departmentId, CancellationToken cancellationToken);

    Task<Result<Department, Error>> GetByAsync(Expression<Func<Department, bool>> predicate, CancellationToken cancellationToken);

    Task<int> UpdateDescendantsPathAsync(
        DepartmentPath oldPath,
        DepartmentPath newPath,
        short depthDelta,
        CancellationToken cancellationToken);

    void SetRowVersion(Department department, uint rowVersion);
}