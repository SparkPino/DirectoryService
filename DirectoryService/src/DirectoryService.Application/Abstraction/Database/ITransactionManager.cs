using System.Data;
using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Application.Abstraction.Database;

public interface ITransactionManager
{
    Task<Result<ITransactionScope, Errors>> BeginTransactionAsync(
        CancellationToken cancellationToken = default,
        IsolationLevel? transactionLevel = null);

    Task<UnitResult<Error>> SaveChangesAsync(CancellationToken cancellationToken);

    Task<UnitResult<Error>> ExecuteAsync(Func<Task> operation);
}