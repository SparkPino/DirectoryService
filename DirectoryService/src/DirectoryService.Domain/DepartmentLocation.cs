namespace DirectoryService.Domain;

public class DepartmentLocation
{
    // EF Core
    private DepartmentLocation()
    {
    }

    public DepartmentLocation(Guid departmentsLocationId, Guid departmentId, Guid locationId)
    {
        DepartmentsLocationId = departmentsLocationId;
        DepartmentId = departmentId;
        LocationId = locationId;
    }

    public Guid DepartmentsLocationId { get; }

    public Guid DepartmentId { get; private set; }

    public Guid LocationId { get; private set; }
}