namespace DirectoryService.Contracts.Department;

public sealed record DepartmentDto(
    string Name,
    string Identifier,
    IEnumerable<Guid> LocationIds,
    uint RowVersion,
    Guid? ParentId = null);
    
public sealed class DepartmentRow
{
    public string Name { get; init; } = null!;
    public string Identifier { get; init; } = null!;
    public long RowVersion { get; init; }
    public Guid? ParentId { get; init; }
    public List<Guid> LocationIds { get; init; } = [];
}