using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain;

public class Location
{
    private readonly List<DepartmentLocation> _departmentsLocations;

    public Guid Id { get; private set; }

    public LocationName Name { get; private set; }

    public Address Address { get; private set; }

    public LocationTimeZone TimeZone { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public IReadOnlyList<DepartmentLocation> DepartmentsLocations => _departmentsLocations;

    private Location(
        Guid? id,
        LocationName name,
        Address address,
        LocationTimeZone timeZone
    )
    {
        Id = id ?? Guid.NewGuid();
        Name = name;
        Address = address;
        TimeZone = timeZone;
        CreatedAt = DateTimeOffset.UtcNow;
        IsActive = true;
    }

    private Location()
    {
    }


    public static Result<Location, string> Create(
        IEnumerable<DepartmentLocation> departments,
        LocationName name, Address adress,
        LocationTimeZone timeZone)
    {
        var location = new Location(Guid.NewGuid(), name, adress, timeZone);

        foreach (var department in departments ?? [])
        {
            location.AddDepartment(department);
        }

        return location;
    }

    public UnitResult<string> AddDepartment(DepartmentLocation? department)
    {
        if (department == null) return UnitResult.Failure("Не может быть null");
        if (_departmentsLocations.Any(d => d.DepartmentId == department.DepartmentId))
            return UnitResult.Failure("Департамент уже добавлен");

        _departmentsLocations.Add(department);
        UpdatedAt = DateTimeOffset.UtcNow;
        return UnitResult.Success<string>();
    }
}