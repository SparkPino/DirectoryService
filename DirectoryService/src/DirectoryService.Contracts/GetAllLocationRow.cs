namespace DirectoryService.Contracts.Locations;

public sealed class GetAllLocationRow
{
    public string Name { get; init; }

    public string Address { get; init; }

    public DateTimeOffset CreatedAt { get; init; }

    public int AttachDepartmentCount { get; init; }

    public long TotalCount { get; init; }
}