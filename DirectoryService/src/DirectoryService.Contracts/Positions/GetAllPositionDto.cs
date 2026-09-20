namespace DirectoryService.Contracts.Positions;

public sealed class GetAllPositionDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = null!;

    public string? Description { get; init; }

    public DateTimeOffset CreatedAt { get; init; }

    public int AttachDepartmentCount { get; init; }
}
