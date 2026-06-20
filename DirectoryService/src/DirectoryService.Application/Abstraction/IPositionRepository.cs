using CSharpFunctionalExtensions;
using DirectoryService.Domain.Positions;
using Shared;

namespace DirectoryService.Application.Abstraction;

public interface IPositionRepository
{
    Task<Guid> AddAsync(Position position, CancellationToken cancellationToken);

    Task<Result<Position, Error>> GetByIdAsync(Guid positionId, CancellationToken cancellationToken);

    Task<Result<Unit, Error>> DeleteByIdAsync(Guid positionId, CancellationToken cancellationToken);
}