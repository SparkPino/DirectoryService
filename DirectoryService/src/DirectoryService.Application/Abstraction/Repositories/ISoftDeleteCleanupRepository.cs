using CSharpFunctionalExtensions;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Abstraction.Repositories;

public interface ISoftDeleteCleanupRepository
{
    Task<Result<int, Error>> DeleteExpiredDepartmentsBatchAsync(DateTimeOffset olderThan, int batchSize,
        CancellationToken ct);

    Task<Result<int, Error>> DeleteExpiredLocationsBatchAsync(DateTimeOffset olderThan, int batchSize,
        CancellationToken ct);

    Task<Result<int, Error>> DeleteExpiredPositionsBatchAsync(DateTimeOffset olderThan, int batchSize,
        CancellationToken ct);
}