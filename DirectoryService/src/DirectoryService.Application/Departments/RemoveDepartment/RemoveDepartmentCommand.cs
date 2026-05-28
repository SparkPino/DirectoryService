using DirectoryService.Application.Abstraction;

namespace DirectoryService.Application.Departments.RemoveDepartment;

public record RemoveDepartmentCommand(Guid departmentId) : ICommand;