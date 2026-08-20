using Core;

namespace DirectoryService.Application.Departments.Commands.AttachLocationToDepartment;

public record AttachLocationToDepartmentCommand(Guid DepartmentId, Guid LocationId) : ICommand;