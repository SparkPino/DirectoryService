using Core;

namespace DirectoryService.Application.Departments.Commands.DetachPositionFromDepartment;

public record DetachPositionFromDepartmentCommand(Guid DepartmentId, Guid PositionId) : ICommand;