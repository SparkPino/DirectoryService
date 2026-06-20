using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Validations;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;


namespace DirectoryService.Application.Departments.RemoveDepartment;

public class RemoveDepartmentHandler : ICommandHandler<RemoveDepartmentCommand, Unit>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogger<RemoveDepartmentHandler> _logger;
    private readonly IValidator<RemoveDepartmentCommand> _validator;

    public RemoveDepartmentHandler(
        IDepartmentRepository departmentRepository,
        ILogger<RemoveDepartmentHandler> logger,
        IValidator<RemoveDepartmentCommand> validator)
    {
        _departmentRepository = departmentRepository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<Unit, Errors>> Handle(
        RemoveDepartmentCommand command,
        CancellationToken cancellationToken)
    {
        var removeResult = await _departmentRepository.DeleteByIdAsync(command.departmentId, cancellationToken);
        if (removeResult.IsFailure)
        {
            _logger.LogError("Не удалось удалить департамент id:{departmentId}: {Error}", command.departmentId,
                removeResult.Error);

            return removeResult.Error.ToErrors();
        }

        return Result.Success<Unit, Errors>(Unit.Value);
    }
}