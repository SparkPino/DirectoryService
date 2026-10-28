using Core;

namespace DirectoryService.Application.Departments.Commands.DeactivateDepartment;

public record DeactivateDepartmentCommand(Guid DepartmentId) : ICommand;