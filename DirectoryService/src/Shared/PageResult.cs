namespace Shared;

public record PagedResult<T>(
    IReadOnlyCollection<T> Items,
    long TotalCount);