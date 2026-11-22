using Core;
using Core.Database;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction.Repositories;
using Shared;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Departments.Commands.BulkDeactivateDepartment;

public class BulkDeactivateHandler : ICommandHandler<BulkDeactivateCommand, Unit>
{
    private readonly IDepartmentRepository _repository;
    private readonly ITransactionManager _transactionManager;

    public BulkDeactivateHandler(
        IDepartmentRepository repository,
        ITransactionManager transactionManager)
    {
        _repository = repository;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Unit, Errors>> Handle(BulkDeactivateCommand command, CancellationToken cancellationToken)
    {
        var timeNow = DateTimeOffset.UtcNow;

        var transactionResult = await _transactionManager.ExecuteInTransactionAsync<Unit>(
            async (ct) =>
            {
                List<Error> errors = new();
                var departmentTrees = await _repository.GetSubtreesAsync(command.DepartmentIds, ct);
                var missingIds =
                    command.DepartmentIds.Except(departmentTrees.Select(a => a.Key.Id.Id)).ToList();

                foreach (var id in missingIds)
                {
                    errors.Add(Error.Conflict(
                        "bulk.deactivate.not.all.exist",
                        id.ToString(),
                        $"Подразделение с id:{id} не существует"));
                }

                foreach (var item in departmentTrees.OrderByDescending(a => a.Key.Depth))
                {
                    var deactivateResult = item.Key.Deactivate(item.Value, timeNow);
                    if (!deactivateResult.IsSuccess)
                    {
                        errors.Add(Error.Conflict(
                            deactivateResult.Error.Code,
                            item.Key.Id.Id.ToString(),
                            deactivateResult.Error.Message));
                    }
                }

                if (errors.Count > 0) return new Errors(errors);

                var saveResult = await _transactionManager.SaveChangesAsync(ct);
                if (!saveResult.IsSuccess) return saveResult.Error.ToErrors();

                return Unit.Value;
            }, cancellationToken);

        if (transactionResult.IsFailure)
        {
            return transactionResult.Error;
        }

        return transactionResult;
    }
}