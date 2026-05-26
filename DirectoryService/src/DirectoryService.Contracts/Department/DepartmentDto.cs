namespace DirectoryService.Contracts.Department;

public sealed record DepartmentDto(string Name, string Identifier, Guid LocationId);