using Core;

namespace DirectoryService.Application.Departments.Commands.MoveDepartment;

public record MoveDepartmentCommand(Guid DepartmentId, Guid? NewParentId) : ICommand;