using DirectoryService.Domain.DepartmentPositions.ValueObjects;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Positions.ValueObjects;

namespace DirectoryService.Domain.DepartmentPositions;

public sealed class DepartmentPosition
{
    // EF Core
    private DepartmentPosition()
    {
    }

    public DepartmentPosition(DepartmentId departmentId, PositionId positionId, DepartmentPositionId? id = null)
    {
        DepartmentPositionId = id ?? new DepartmentPositionId(Guid.NewGuid());
        DepartmentId = departmentId;
        PositionId = positionId;
    }

    public DepartmentPositionId DepartmentPositionId { get; }

    public DepartmentId DepartmentId { get; private set; }

    public PositionId PositionId { get; private set; }

}