using Core;
using Core.Database;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction.Repositories;
using Shared;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Departments.Commands.BulkSoftDeleteDepartment;

public class BulkSoftDeleteHandler : ICommandHandler<BulkSoftDeleteCommand, Unit>
{
    private readonly IDepartmentRepository _repository;
    private readonly ITransactionManager _transactionManager;

    public BulkSoftDeleteHandler(
        IDepartmentRepository repository,
        ITransactionManager transactionManager)
    {
        _repository = repository;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Unit, Errors>> Handle(
        BulkSoftDeleteCommand command,
        CancellationToken cancellationToken)
    {
        var timeNow = DateTimeOffset.UtcNow;
        var transactionResult = await _transactionManager.ExecuteInTransactionAsync<Unit>(
            async (ct) =>
            {
                List<Error> errors = new();
                var result = await _repository.GetSubtreesAsync(command.DepartmentIds, ct);
                var missingIds = command.DepartmentIds.Except(result.Keys.Select(b => b.Id.Id)).ToList();

                foreach (var departmentId in missingIds)
                {
                    errors.Add(Error.Conflict(
                        "bulk.soft_delete.not.all.exist",
                        departmentId.ToString(),
                        $"Подразделения c id:{departmentId} не существует"));
                }

                foreach (var item in result)
                {
                    var softDeleteResult = item.Key.SoftDelete(item.Value, timeNow);
                    if (!softDeleteResult.IsSuccess)
                    {
                        errors.Add(Error.Conflict(
                            softDeleteResult.Error.Code,
                            item.Key.Id.Id.ToString(),
                            softDeleteResult.Error.Message));
                    }
                }

                if (errors.Count > 0) return new Errors(errors);

                var saveResult = await _transactionManager.SaveChangesAsync(ct);
                if (saveResult.IsFailure) return saveResult.Error.ToErrors();

                return Unit.Value;
            }, cancellationToken);

        if (transactionResult.IsFailure)
        {
            return transactionResult.Error;
        }

        return transactionResult;
    }
}