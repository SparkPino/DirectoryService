using DirectoryService.Application.Abstraction;
using DirectoryService.Contracts.Department;

namespace DirectoryService.Application.Departments.UpdateDepartment;

public record UpdateDepartmentCommand(UpdateDepartmentDto DepartmentDto, Guid Id) : ICommand<Guid>;