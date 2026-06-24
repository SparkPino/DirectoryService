using DirectoryService.Application.Abstraction;

namespace DirectoryService.Application.Departments.Commands.RemoveDepartment;

public record RemoveDepartmentCommand(Guid departmentId) : ICommand;