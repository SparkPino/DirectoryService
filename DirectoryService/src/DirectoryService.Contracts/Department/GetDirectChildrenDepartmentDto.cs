namespace DirectoryService.Contracts.Department;

public class DirectChildDepartmentDto
{
    public Guid Id { get; init; }

    public Guid? ParentId { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Identifier { get; init; } = string.Empty;

    public string Path { get; init; } = string.Empty;

    public int Depth { get; init; }

    public bool IsActive { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }

    public DateTime? DeletedAt { get; init; }

    public bool HasMoreChildren { get; init; }
}

public class DepartmentNodeDto : DirectChildDepartmentDto
{
    public List<DirectChildDepartmentDto> Children { get; init; } = [];
}

public class GetDirectChildrenDepartmentDto
{
    public List<DepartmentNodeDto> Nodes { get; init; } = [];
}