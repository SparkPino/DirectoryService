namespace DirectoryService.Contracts.Department;

public sealed record GetDepartmentDto
{
    public Guid DepartmentId { get; init; }

    public string Path { get; init; }

    public string Name { get; init; }

    public DateTime CreatedAt { get; init; }

    public long TotalCount { get; init; }
}