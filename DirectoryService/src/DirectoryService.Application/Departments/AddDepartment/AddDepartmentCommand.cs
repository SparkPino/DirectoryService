using DirectoryService.Application.Abstraction;
using DirectoryService.Contracts.Department;

namespace DirectoryService.Application.Departments.AddDepartment;

public record AddDepartmentCommand(DepartmentDto DepartmentDto) : ICommand;