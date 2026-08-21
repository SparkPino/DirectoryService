using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction.Repositories;
using DirectoryService.Infrastructure.Postgres.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

public class SoftDeleteCleanupRepository : ISoftDeleteCleanupRepository
{
    private readonly DirectoryServiceDbContext _dbContext;
    private readonly ILogger<SoftDeleteCleanupRepository> _logger;

    public SoftDeleteCleanupRepository(DirectoryServiceDbContext dbContext, ILogger<SoftDeleteCleanupRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<int, Error>> DeleteExpiredDepartmentsBatchAsync(DateTimeOffset olderThan, int batchSize,
        CancellationToken ct)
    {
        try
        {
            var ids = await _dbContext.Departments
                .IgnoreQueryFilters()
                .Where(d => d.IsActive == false && d.DeletedAt < olderThan)
                .OrderBy(d => d.DeletedAt)
                .Take(batchSize)
                .Select(d => d.Id)
                .ToListAsync(ct);

            if (ids.Count == 0) return 0;

            return await _dbContext.Departments
                .IgnoreQueryFilters()
                .Where(d => ids.Contains(d.Id))
                .ExecuteDeleteAsync(ct);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return DbExceptionMapper.Map(ex, _logger);
        }
    }


    public async Task<Result<int, Error>> DeleteExpiredLocationsBatchAsync(DateTimeOffset olderThan, int batchSize,
        CancellationToken ct)
    {
        try
        {
            var ids = await _dbContext.Locations
                .IgnoreQueryFilters()
                .Where(d => d.IsActive == false && d.DeletedAt < olderThan)
                .OrderBy(d => d.DeletedAt)
                .Take(batchSize)
                .Select(d => d.Id)
                .ToListAsync(ct);

            if (ids.Count == 0) return 0;

            return await _dbContext.Locations
                .IgnoreQueryFilters()
                .Where(d => ids.Contains(d.Id))
                .ExecuteDeleteAsync(ct);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return DbExceptionMapper.Map(ex, _logger);
        }
    }

    public async Task<Result<int, Error>> DeleteExpiredPositionsBatchAsync(DateTimeOffset olderThan, int batchSize,
        CancellationToken ct)
    {
        try
        {
            var ids = await _dbContext.Positions
                .IgnoreQueryFilters()
                .Where(d => d.IsActive == false && d.DeletedAt < olderThan)
                .OrderBy(d => d.DeletedAt)
                .Take(batchSize)
                .Select(d => d.Id)
                .ToListAsync(ct);

            if (ids.Count == 0) return 0;

            return await _dbContext.Positions
                .IgnoreQueryFilters()
                .Where(d => ids.Contains(d.Id))
                .ExecuteDeleteAsync(ct);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return DbExceptionMapper.Map(ex, _logger);
        }
    }
}