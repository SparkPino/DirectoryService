using CSharpFunctionalExtensions;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Shared;
using Shared;

namespace DirectoryService.Application.Abstraction;

public interface ILocationRepository
{
    Task<Result<IReadOnlyList<Location>, Errors>> GetPaged(int page, int pageSize, CancellationToken cancellationToken);
    Task<Result<Guid, Error>> AddAsync(Location location, CancellationToken cancellationToken);

    Task<int> SaveAsync(CancellationToken cancellationToken);

    Task<Result<Guid, Error>> DeleteAsync(Guid locationId, CancellationToken cancellationToken);

    Task<Result<Location, Error>> GetByIdAsync(Guid locationId, CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<Location>, Error>> GetByIdsAsync(
        IEnumerable<Guid> locationIds,
        CancellationToken cancellationToken);
}