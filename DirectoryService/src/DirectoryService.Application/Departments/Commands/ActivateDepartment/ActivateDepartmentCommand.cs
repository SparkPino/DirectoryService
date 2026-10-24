using Core;

namespace DirectoryService.Application.Departments.Commands.ActivateDepartment;

public record ActivateDepartmentCommand(Guid DepartmentId) : ICommand;