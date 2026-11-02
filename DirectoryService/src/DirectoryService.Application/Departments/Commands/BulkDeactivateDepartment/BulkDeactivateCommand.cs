using Core;

namespace DirectoryService.Application.Departments.Commands.BulkDeactivateDepartment;

public sealed record BulkDeactivateCommand(List<Guid> DepartmentIds) : ICommand;