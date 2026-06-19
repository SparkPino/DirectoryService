using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Application.Abstraction;

public interface ITransactionManager
{
    Task<Result<ITransactionScope, Errors>> TransactionBegin(CancellationToken cancellationToken);

    Task<UnitResult<Error>> SaveChangesAsync(CancellationToken cancellationToken);

    Task<UnitResult<Error>> ExecuteAsync(Func<Task> operation);
}