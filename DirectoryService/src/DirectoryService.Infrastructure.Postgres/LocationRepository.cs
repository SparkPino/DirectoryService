using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Locations;
using DirectoryService.Application.Locations.Failures;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres;

public class LocationRepository : ILocationRepository
{
    private readonly DirectoryServiceDbContext _context;
    private readonly ILogger<LocationRepository> _logger;

    public LocationRepository(DirectoryServiceDbContext context, ILogger<LocationRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> AddAsync(Location location, CancellationToken cancellationToken)
    {
        await _context.AddAsync(location, cancellationToken);
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return location.Id;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            var errorDescription = Errors.LocationErrors.Concurrency;
            _logger.LogError(ex.InnerException ?? ex,
                "Конфликт параллельного доступа при добавлении локации {LocationId}", location.Id);
            return errorDescription;
        }
        catch (DbUpdateException ex)
        {
            var errorDescription = Errors.LocationErrors.Database;
            _logger.LogError(ex.InnerException ?? ex, "Ошибка базы данных при добавлении локации {LocationId}",
                location.Id);
            return errorDescription;
        }
    }

    public Task<Result<Guid, Error>> SaveAsync(Location location, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<Result<Guid, Error>> DeleteAsync(Guid questionId, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<Result<Guid, Error>> GetByIdAsync(Guid questionId, CancellationToken cancellationToken) =>
        throw new NotImplementedException();
}