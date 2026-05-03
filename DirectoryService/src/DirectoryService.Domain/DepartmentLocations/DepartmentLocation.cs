using DirectoryService.Domain.DepartmentLocations.ValueObjects;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;

namespace DirectoryService.Domain.DepartmentLocations;

public sealed class DepartmentLocation
{
    // EF Core
    private DepartmentLocation()
    {
    }

    public DepartmentLocation(DepartmentId departmentId, LocationId locationId, DepartmentLocationId? departmentsLocationId = null)
    {
        DepartmentsLocationId = departmentsLocationId ?? new DepartmentLocationId(Guid.NewGuid());
        DepartmentId = departmentId;
        LocationId = locationId;
    }

    public DepartmentLocationId DepartmentsLocationId { get; }

    public LocationId LocationId { get; private set; }

    public DepartmentId DepartmentId { get; private set; }

}