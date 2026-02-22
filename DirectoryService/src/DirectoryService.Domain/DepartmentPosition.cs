namespace DirectoryService.Domain;

public class DepartmentPosition
{
    public Guid Id { get; set; }

    public Guid DepartmentId { get; set; }

    public Guid PositionId { get; set; }
}