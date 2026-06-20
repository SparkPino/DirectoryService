using DirectoryService.Application.Abstraction;

namespace DirectoryService.Application.Departments.AttachPositionToDepartment;

public record AttachPositionToDepartmentCommand(Guid DepartmentId, Guid PositionId) : ICommand;