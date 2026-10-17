using Core;
using Core.Database;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction.Repositories;
using DirectoryService.Application.Departments.Failures;
using Shared;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Departments.Commands.SoftDeleteDepartment;

public class SoftDeleteDepartmentHandler : ICommandHandler<SoftDeleteDepartmentCommand, Unit>
{
    private readonly IDepartmentRepository _repository;
    private readonly ITransactionManager _transactionManager;

    public SoftDeleteDepartmentHandler(IDepartmentRepository repository, ITransactionManager transactionManager)
    {
        _repository = repository;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Unit, Errors>> Handle(SoftDeleteDepartmentCommand command,
        CancellationToken cancellationToken)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;

        var subTreeWithRoots = await _repository.GetSubtreesAsync([command.Id.Id], cancellationToken);

        if (subTreeWithRoots.Count == 0)
            return DepartmentError.NotFound(command.Id.Id).ToErrors();

        foreach (var subTree in subTreeWithRoots)
        {
            subTree.Key.SoftDelete(subTree.Value, now);
        }

        var result = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (result.IsFailure) return result.Error.ToErrors();


        return Unit.Value;
    }
}