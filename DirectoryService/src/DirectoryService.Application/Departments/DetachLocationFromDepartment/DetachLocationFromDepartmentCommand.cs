using DirectoryService.Application.Abstraction;

namespace DirectoryService.Application.Departments.DetachLocationFromDepartment;

public record DetachLocationFromDepartmentCommand(Guid DepartmentId, Guid LocationId) : ICommand;