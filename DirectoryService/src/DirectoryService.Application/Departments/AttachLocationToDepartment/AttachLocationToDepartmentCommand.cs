using DirectoryService.Application.Abstraction;

namespace DirectoryService.Application.Departments.AttachLocationToDepartment;

public record AttachLocationToDepartmentCommand(Guid DepartmentId, Guid LocationId) : ICommand;