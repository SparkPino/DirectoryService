using DirectoryService.Application.Abstraction;

namespace DirectoryService.Application.Departments.DetachPositionFromDepartment;

public record DetachPositionFromDepartmentCommand(Guid DepartmentId, Guid PositionId) : ICommand;