using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Locations.Failures;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

public class LocationRepository : ILocationRepository
{
    private readonly DirectoryServiceDbContext _context;
    private readonly ILogger<LocationRepository> _logger;

    public LocationRepository(DirectoryServiceDbContext context, ILogger<LocationRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Location>> GetPaged(
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var locations = await _context.Locations
            .AsNoTracking()
            .OrderBy(l => l.Name).ThenBy(l => l.Id) // <- must have 
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return locations;
    }

    public async Task<Result<Guid, Error>> AddAsync(Location location, CancellationToken cancellationToken)
    {
        await _context.AddAsync(location, cancellationToken);
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return location.Id.Id;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            var errorDescription = LocationErrors.Concurrency;
            _logger.LogError(ex.InnerException ?? ex,
                "Конфликт параллельного доступа при добавлении локации {LocationId}", location.Id);
            return errorDescription;
        }
        catch (DbUpdateException ex)
        {
            var errorDescription = LocationErrors.Database;
            _logger.LogError(ex.InnerException ?? ex, "Ошибка базы данных при добавлении локации {LocationId}",
                location.Id);
            return errorDescription;
        }
    }

    public async Task<int> SaveAsync(CancellationToken cancellationToken) =>
        await _context.SaveChangesAsync(cancellationToken);


    public Task<Result<Guid, Error>> DeleteAsync(Guid locationId, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public async Task<Result<Location, Error>> GetByIdAsync(Guid locationId, CancellationToken cancellationToken)
    {
        var correctLocationId = new LocationId(locationId);

        var location = await _context.Locations
            .FirstOrDefaultAsync(l => l.Id == correctLocationId, cancellationToken: cancellationToken);
        if (location == null)
        {
            _logger.LogWarning("Локация с id:{LocationId} не найдена.", locationId);
            return LocationErrors.NotFound(locationId);
        }

        return location;
    }

    public async Task<Result<IReadOnlyList<Location>, Error>> GetByIdsAsync(
        IEnumerable<Guid> locationId,
        CancellationToken cancellationToken)
    {
        var idList = locationId.Select(g => new LocationId(g)).ToList();
        var locations = await _context.Locations
            .Where(l => idList.Contains(l.Id))
            .ToListAsync(cancellationToken: cancellationToken);


        if (locations.Count != idList.Count)
        {
            var foundsId = locations.Select(l => new LocationId(l.Id.Id));
            var missingId = idList.Except(foundsId).ToList();

            _logger.LogWarning("Локация с id:{LocationId} не найдена.", string.Join(", ", missingId));
            return LocationErrors.NotFoundMany(missingId.Select(l => l.Id));
        }

        return locations;
    }
}