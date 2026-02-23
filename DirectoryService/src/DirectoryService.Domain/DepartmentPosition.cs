namespace DirectoryService.Domain;

public class DepartmentPosition
{
    public required Guid Id { get; init; }

    public Guid DepartmentId { get; set; }

    public Guid PositionId { get; set; }
}