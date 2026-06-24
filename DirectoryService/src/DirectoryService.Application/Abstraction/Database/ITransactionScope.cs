using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Application.Abstraction.Database;

public interface ITransactionScope : IDisposable
{
    UnitResult<Error> Commit();

    UnitResult<Error> Rollback();
}