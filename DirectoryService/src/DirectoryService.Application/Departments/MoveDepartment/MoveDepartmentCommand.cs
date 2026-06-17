using DirectoryService.Application.Abstraction;

namespace DirectoryService.Application.Departments.MoveDepartment;

public record MoveDepartmentCommand(Guid DepartmentId, Guid? NewParentId) : ICommand;