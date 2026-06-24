using DirectoryService.Application.Abstraction;

namespace DirectoryService.Application.Departments.Commands.AttachLocationToDepartment;

public record AttachLocationToDepartmentCommand(Guid DepartmentId, Guid LocationId) : ICommand;