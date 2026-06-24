using DirectoryService.Application.Abstraction;

namespace DirectoryService.Application.Departments.Commands.AttachPositionToDepartment;

public record AttachPositionToDepartmentCommand(Guid DepartmentId, Guid PositionId) : ICommand;