using DirectoryService.Application.Abstraction;
using Shared;


namespace DirectoryService.Application.Departments.RemoveDepartment;

public record RemoveDepartmentCommand(Guid departmentId) : ICommand<Unit>;