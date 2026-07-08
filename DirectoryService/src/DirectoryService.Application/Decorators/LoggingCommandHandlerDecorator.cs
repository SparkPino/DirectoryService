using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Decorators;

public class LoggingCommandHandlerDecorator<TCommand, TResponse> : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    private readonly ICommandHandler<TCommand, TResponse> _inner;
    private readonly ILogger<TCommand> _logger;

    public LoggingCommandHandlerDecorator(
        ICommandHandler<TCommand, TResponse> inner,
        ILogger<TCommand> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task<Result<TResponse, Errors>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        var commandName = typeof(TCommand).Name;

        _logger.LogInformation("Обработка команды {CommandName}: {@Command}", commandName, command);

        var result = await _inner.Handle(command, cancellationToken);

        if (result.IsFailure)
        {
            _logger.LogWarning("Команда {CommandName} завершилась с ошибкой: {@Errors}", commandName, result.Error);
        }
        else
        {
            _logger.LogInformation("Команда {CommandName} успешно обработана. Результат: {@Result}", commandName,
                result.Value);
        }

        return result;
    }
}