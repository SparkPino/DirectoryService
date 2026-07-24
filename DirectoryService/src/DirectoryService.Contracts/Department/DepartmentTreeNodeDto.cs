namespace DirectoryService.Contracts.Department;

public class DepartmentTreeNodeDto
{
    public Guid Id { get; init; }

    public Guid MatchId { get; init; }

    public Guid? ParentId { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Identifier { get; init; } = string.Empty;

    public string Path { get; init; } = string.Empty;

    public int Depth { get; init; }

    public bool HasChildren { get; init; }

    public int? ChildrenCount { get; init; }
}