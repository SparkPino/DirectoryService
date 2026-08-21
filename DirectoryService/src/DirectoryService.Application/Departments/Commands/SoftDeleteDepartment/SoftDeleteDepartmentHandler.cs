using Core;
using Core.Database;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction.Repositories;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Departments.Commands.SoftDeleteDepartment;

public class SoftDeleteDepartmentHandler : ICommandHandler<SoftDeleteDepartmentCommand, Guid>
{
    private readonly IDepartmentRepository _repository;
    private readonly ITransactionManager _transactionManager;

    public SoftDeleteDepartmentHandler(IDepartmentRepository repository, ITransactionManager transactionManager)
    {
        _repository = repository;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Guid, Errors>> Handle(SoftDeleteDepartmentCommand command,
        CancellationToken cancellationToken)
    {
        var department = await _repository.GetByAsync(d => d.Id == command.Id, cancellationToken);
        if (department.IsFailure)
        {
            return department.Error.ToErrors();
        }

        department.Value.SoftDelete();
        var saveChangesResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveChangesResult.IsFailure)
        {
            return saveChangesResult.Error.ToErrors();
        }

        return department.Value.Id.Id;
    }
}