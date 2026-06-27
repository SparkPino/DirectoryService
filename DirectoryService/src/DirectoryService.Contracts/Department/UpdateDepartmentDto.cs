namespace DirectoryService.Contracts.Department;

public record UpdateDepartmentDto(string? Name, string? Identifier, uint RowVersion);