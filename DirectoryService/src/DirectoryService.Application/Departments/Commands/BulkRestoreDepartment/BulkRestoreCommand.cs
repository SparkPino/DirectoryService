using Core;

namespace DirectoryService.Application.Departments.Commands.BulkRestoreDepartment;

public sealed record BulkRestoreCommand(List<Guid> DepartmentIds) : ICommand;
 