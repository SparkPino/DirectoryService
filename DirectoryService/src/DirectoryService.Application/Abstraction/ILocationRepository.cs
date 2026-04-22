using CSharpFunctionalExtensions;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Application.Abstraction;

public interface ILocationRepository
{
    Task<Result<Guid, Error>> AddAsync(Location location, CancellationToken cancellationToken);

    Task<Result<Guid, Error>> SaveAsync(Location location, CancellationToken cancellationToken); //update, save

    Task<Result<Guid, Error>> DeleteAsync(Guid questionId, CancellationToken cancellationToken);

    Task<Result<Guid, Error>> GetByIdAsync(Guid questionId, CancellationToken cancellationToken);
}