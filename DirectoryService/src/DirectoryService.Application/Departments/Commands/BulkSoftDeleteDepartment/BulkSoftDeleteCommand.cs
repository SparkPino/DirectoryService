using Core;

namespace DirectoryService.Application.Departments.Commands.BulkSoftDeleteDepartment;

public sealed record BulkSoftDeleteCommand(List<Guid> DepartmentIds) : ICommand;