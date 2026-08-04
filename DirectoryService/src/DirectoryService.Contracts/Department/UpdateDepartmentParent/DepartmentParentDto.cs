namespace DirectoryService.Contracts.Department.UpdateDepartmentParent;

public record DepartmentParentDto(
    Guid Id,
    Guid? ParentId,
    string Path,
    int Depth,
    DateTimeOffset? UpdatedAt);