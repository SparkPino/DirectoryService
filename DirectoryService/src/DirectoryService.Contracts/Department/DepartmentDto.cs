namespace DirectoryService.Contracts.Department;

public sealed record DepartmentDto(
    string Name,
    string Identifier,
    IEnumerable<Guid> LocationIds,
    Guid? ParentId = null);