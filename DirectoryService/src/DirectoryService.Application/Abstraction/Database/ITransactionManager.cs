using System.Data;
using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Application.Abstraction.Database;

public interface ITransactionManager
{
    Task<UnitResult<Error>> SaveChangesAsync(CancellationToken cancellationToken);

    // Оборачивает всю связку "открыть транзакцию -> сделать работу -> закоммитить" в
    // CreateExecutionStrategy().ExecuteAsync, потому что EF Core запрещает вручную
    // открытые транзакции (Database.BeginTransactionAsync) при включённом
    // EnableRetryOnFailure — стратегия ретраев обязана владеть всей операцией целиком,
    // чтобы при транзиентном сбое безопасно повторить её с нуля.
    Task<Result<TResult, Errors>> ExecuteInTransactionAsync<TResult>(
        Func<CancellationToken, Task<Result<TResult, Errors>>> operation,
        CancellationToken cancellationToken,
        IsolationLevel? isolationLevel = null);
}