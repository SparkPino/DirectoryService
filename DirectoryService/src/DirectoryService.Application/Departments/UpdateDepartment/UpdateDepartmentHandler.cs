using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Domain.Departments.ValueObjects;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Departments.UpdateDepartment;

public class UpdateDepartmentHandler : ICommandHandler<UpdateDepartmentCommand, Guid>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogger<UpdateDepartmentHandler> _logger;

    public UpdateDepartmentHandler(IDepartmentRepository departmentRepository, ILogger<UpdateDepartmentHandler> logger)
    {
        _departmentRepository = departmentRepository;
        _logger = logger;
    }

    public async Task<Result<Guid, Errors>> Handle(UpdateDepartmentCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Обработка UpdateDepartmentCommand id:{departmentId}", command.Id);

        var departmentResult = await _departmentRepository.GetByIdAsync(command.Id, cancellationToken);
        if (departmentResult.IsFailure) return departmentResult.Error.ToErrors();
        DepartmentName? name = null;
        if (command.DepartmentDto.Name != null)
        {
            var newNameResult = DepartmentName.Create(command.DepartmentDto.Name);
            if (newNameResult.IsFailure) return newNameResult.Error;
            name = newNameResult.Value;
        }

        DepartmentIdentifier? identifier = null;
        if (command.DepartmentDto.Identifier != null)
        {
            var newDepartmentIdentifierResult = DepartmentIdentifier.Create(command.DepartmentDto.Identifier);
            if (newDepartmentIdentifierResult.IsFailure) return newDepartmentIdentifierResult.Error;
            identifier = newDepartmentIdentifierResult.Value;
        }


        var updateDepartmentResult =
            departmentResult.Value.UpdateDepartment(name, identifier);
        if (updateDepartmentResult.IsFailure) return updateDepartmentResult.Error;
        await _departmentRepository.SaveAsync(cancellationToken);

        return command.Id;
    }
}