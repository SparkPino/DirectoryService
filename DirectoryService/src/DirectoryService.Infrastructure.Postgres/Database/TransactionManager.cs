using System.Data.Common;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Infrastructure.Postgres.Database;

public class TransactionManager : ITransactionManager
{
    private readonly DirectoryServiceDbContext _dbContext;
    private readonly ILogger<TransactionManager> _logger;
    private readonly ILoggerFactory _loggerFactory;

    public TransactionManager(DirectoryServiceDbContext dbContext, ILogger<TransactionManager> logger,
        ILoggerFactory loggerFactory)
    {
        _dbContext = dbContext;
        _logger = logger;
        _loggerFactory = loggerFactory;
    }

    public async Task<Result<ITransactionScope, Errors>> TransactionBegin(CancellationToken cancellationToken)
    {
        try
        {
            var transactionResult = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            //Стандартный способ создание логера в рантайме
            var transactionScopeLogger = _loggerFactory.CreateLogger<TransactionScope>();

            var transactionScope = new TransactionScope(transactionResult.GetDbTransaction(), transactionScopeLogger);
            return transactionScope;
        }
        catch (Exception ex)
        {
            return DbExceptionMapper.Map(ex, _logger).ToErrors();
        }
    }

    public async Task<UnitResult<Error>> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return UnitResult.Success<Error>();
        }
        catch (Exception e)
        {
            return DbExceptionMapper.Map(e, _logger);
        }
    }

    public async Task<UnitResult<Error>> ExecuteAsync(Func<Task> operation)
    {
        try
        {
            await operation();
            return UnitResult.Success<Error>();
        }
        catch (Exception e)
        {
            return DbExceptionMapper.Map(e, _logger);
        }
    }
}