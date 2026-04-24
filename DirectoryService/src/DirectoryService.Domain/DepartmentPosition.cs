using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Positions;

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

    public Department? Department { get; private set; }

    public Position? Position { get; private set; }
}