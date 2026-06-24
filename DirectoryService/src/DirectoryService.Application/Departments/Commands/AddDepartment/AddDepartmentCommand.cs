using DirectoryService.Application.Abstraction;
using DirectoryService.Contracts.Department;

namespace DirectoryService.Application.Departments.Commands.AddDepartment;

public record AddDepartmentCommand(DepartmentDto DepartmentDto) : ICommand;