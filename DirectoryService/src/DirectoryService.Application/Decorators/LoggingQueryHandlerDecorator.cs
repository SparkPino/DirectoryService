using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Decorators;

public class LoggingQueryHandlerDecorator<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    private readonly IQueryHandler<TQuery, TResponse> _inner;
    private readonly ILogger<LoggingQueryHandlerDecorator<TQuery, TResponse>> _logger;

    public LoggingQueryHandlerDecorator(
        IQueryHandler<TQuery, TResponse> inner,
        ILogger<LoggingQueryHandlerDecorator<TQuery, TResponse>> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task<Result<TResponse, Errors>> Handle(TQuery query, CancellationToken cancellationToken)
    {
        var queryName = typeof(TQuery).Name;

        _logger.LogInformation("Обработка запроса {QueryName}: {@Query}", queryName, query);

        var result = await _inner.Handle(query, cancellationToken);

        if (result.IsFailure)
        {
            _logger.LogWarning("Запрос {QueryName} завершился с ошибкой: {@Errors}", queryName, result.Error);
        }
        else
        {
            _logger.LogInformation("Запрос {QueryName} успешно обработан", queryName);
        }

        return result;
    }
}