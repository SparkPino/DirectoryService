using DirectoryService.Domain.Locations;

namespace DirectoryService.Application.Abstraction;

public interface ILocationRepository
{
    Task<Guid> AddAsync(Location location, CancellationToken cancellationToken);

    Task<Guid> SaveAsync(Location location, CancellationToken cancellationToken);  //update, save

    Task<Guid> DeleteAsync(Guid questionId, CancellationToken cancellationToken);

    Task<Guid> GetByIdAsync(Guid questionId, CancellationToken cancellationToken);
}