namespace DirectoryService.Domain;

public class DepartmentPosition
{
    // EF Core
    private DepartmentPosition()
    {
    }

    public DepartmentPosition(Guid id, Guid departmentId, Guid positionId)
    {
        DepartmentPositionId = id;
        DepartmentId = departmentId;
        PositionId = positionId;
    }

    public Guid DepartmentPositionId { get; }

    public Guid DepartmentId { get; private set; }

    public Guid PositionId { get; private set; }
}