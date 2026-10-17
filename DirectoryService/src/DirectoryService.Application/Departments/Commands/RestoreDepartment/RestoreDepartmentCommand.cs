using Core;

namespace DirectoryService.Application.Departments.Commands.RestoreDepartment;

public record RestoreDepartmentCommand(Guid Departmentid) : ICommand;