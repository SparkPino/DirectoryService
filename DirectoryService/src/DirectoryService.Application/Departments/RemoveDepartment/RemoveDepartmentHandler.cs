using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Departments.RemoveDepartment;

public class RemoveDepartmentHandler : ICommandHandler<RemoveDepartmentCommand, Unit>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogger<RemoveDepartmentHandler> _logger;

    public RemoveDepartmentHandler(IDepartmentRepository departmentRepository, ILogger<RemoveDepartmentHandler> logger)
    {
        _departmentRepository = departmentRepository;
        _logger = logger;
    }

    public async Task<Result<Unit, Errors>> Handle(
        RemoveDepartmentCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Обработка RemoveDepartmentCommand id:{departmentId}", command.departmentId);

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