using System.Diagnostics;
using Core;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Decorators.Queries;

internal sealed class QueryLoggingDecorator<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery
{
    private readonly IQueryHandler<TQuery, TResponse> _inner;
    private readonly ILogger<TQuery> _logger;

    public QueryLoggingDecorator(
        IQueryHandler<TQuery, TResponse> inner,
        ILogger<TQuery> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task<Result<TResponse, Errors>> Handle(TQuery query, CancellationToken cancellationToken)
    {
        string queryName = typeof(TQuery).Name;
        long start = Stopwatch.GetTimestamp();
        _logger.LogInformation("Starting query {Query}", queryName);

        var result = await _inner.Handle(query, cancellationToken);
        double elapsedMs = Stopwatch.GetElapsedTime(start).TotalMilliseconds;

        if (result.IsFailure)
        {
            _logger.LogWarning(
                "{Query} failed in {ElapsedMs} ms: {@Errors}", queryName, elapsedMs, result.Error);
            return result;
        }

        _logger.LogInformation("{Query} completed successfully in {ElapsedMs} ms", queryName, elapsedMs);
        return result;
    }
}