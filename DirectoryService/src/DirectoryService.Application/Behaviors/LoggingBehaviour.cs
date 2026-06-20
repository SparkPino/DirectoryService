using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Behaviors;

public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse, Errors>>
    where TRequest : ICommand<TResponse>
{
    private readonly ILogger _logger;

    public LoggingBehaviour(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger(typeof(TRequest).Name);
    }

    public async Task<Result<TResponse, Errors>> Handle(TRequest request,
        RequestHandlerDelegate<Result<TResponse, Errors>> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Обработка команды {CommandName}: {@Command}", typeof(TRequest).Name, request);

        var result = await next(cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogWarning("Команда {CommandName} завершилась с ошибкой: {@Errors}", typeof(TRequest).Name,
                result.Error);
            return result.Error; 
        }

        _logger.LogInformation("Обработка команды {CommandName}: {@Command} завершена успешно", typeof(TRequest).Name,
            request);
        return result;
    }
}