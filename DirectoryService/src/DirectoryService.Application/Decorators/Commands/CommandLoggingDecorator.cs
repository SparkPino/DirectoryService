using System.Diagnostics;
using Core;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Decorators.Commands;

internal sealed class CommandLoggingDecorator<TCommand, TResponse> : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand
{
    private readonly ICommandHandler<TCommand, TResponse> _inner;
    private readonly ILogger<TCommand> _logger;

    public CommandLoggingDecorator(
        ICommandHandler<TCommand, TResponse> inner,
        ILogger<TCommand> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task<Result<TResponse, Errors>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        string commandName = typeof(TCommand).Name;
        long start = Stopwatch.GetTimestamp();
        _logger.LogInformation("Starting command {Command}", commandName);

        var result = await _inner.Handle(command, cancellationToken);
        double elapsedMs = Stopwatch.GetElapsedTime(start).TotalMilliseconds;

        if (result.IsFailure)
        {
            _logger.LogWarning(
                "{Command} failed in {ElapsedMs} ms: {@Errors}", commandName, elapsedMs, result.Error);
            return result;
        }

        _logger.LogInformation("{Command} completed successfully in {ElapsedMs} ms", commandName, elapsedMs);
        return result;
    }
}