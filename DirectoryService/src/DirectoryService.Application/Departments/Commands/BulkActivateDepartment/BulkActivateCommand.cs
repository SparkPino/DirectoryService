using Core;

namespace DirectoryService.Application.Departments.Commands.BulkActivateDepartment;

public sealed record BulkActivateCommand(List<Guid> DepartmentIds) : ICommand;


