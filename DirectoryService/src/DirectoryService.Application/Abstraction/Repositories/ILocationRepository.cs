using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Locations;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Abstraction.Repositories;

public interface ILocationRepository
{
    Task<Guid> AddAsync(Location location, CancellationToken cancellationToken);

    Task<Result<Guid, Error>> DeleteAsync(Guid locationId, CancellationToken cancellationToken);

    Task<Result<Location, Error>> GetByIdAsync(Guid locationId, CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<Location>, Error>> GetByIdsAsync(IEnumerable<Guid> locationIds,
        CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<Guid>, Error>> GetExistingIdsAsync(IEnumerable<Guid> locationId,
        CancellationToken cancellationToken);

    Task<Result<Location, Error>> GetByAsync(Expression<Func<Location, bool>> predicate,
        CancellationToken cancellationToken);
}