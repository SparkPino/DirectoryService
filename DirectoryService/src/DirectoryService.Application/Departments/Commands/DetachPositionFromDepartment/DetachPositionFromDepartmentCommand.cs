using DirectoryService.Application.Abstraction;

namespace DirectoryService.Application.Departments.Commands.DetachPositionFromDepartment;

public record DetachPositionFromDepartmentCommand(Guid DepartmentId, Guid PositionId) : ICommand;