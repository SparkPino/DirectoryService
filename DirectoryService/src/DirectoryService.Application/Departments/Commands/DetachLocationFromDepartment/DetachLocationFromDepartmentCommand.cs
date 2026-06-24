using DirectoryService.Application.Abstraction;

namespace DirectoryService.Application.Departments.Commands.DetachLocationFromDepartment;

public record DetachLocationFromDepartmentCommand(Guid DepartmentId, Guid LocationId) : ICommand;