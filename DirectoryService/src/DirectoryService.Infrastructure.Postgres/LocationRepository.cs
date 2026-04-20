using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Locations;
using DirectoryService.Domain.Locations;

namespace DirectoryService.Infrastructure.Postgres;

public class LocationRepository : ILocationRepository
{
    private readonly DirectoryServiceDbContext _context;

    public LocationRepository(DirectoryServiceDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> AddAsync(Location location, CancellationToken cancellationToken)
    {
        await _context.AddAsync(location, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return location.Id;
    }

    public Task<Guid> SaveAsync(Location location, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<Guid> DeleteAsync(Guid questionId, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<Guid> GetByIdAsync(Guid questionId, CancellationToken cancellationToken) =>
        throw new NotImplementedException();
}