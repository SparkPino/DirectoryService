using Core;
using DirectoryService.Contracts.Department.UpdateDepartmentParent;

namespace DirectoryService.Application.Departments.Commands.UpdateDepartmentParent;

public record UpdateDepartmentParentCommand(Guid Id, UpdateDepartmentParentParentDto? ParentId) : ICommand;