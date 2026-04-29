using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Shared.ValueObjects;

namespace DirectoryService.Domain.Locations;

public class Location
{
    private readonly List<DepartmentLocation> _departmentsLocations = [];

    public Guid Id { get; private set; }

    public LocationName Name { get; private set; }

    public Address Address { get; private set; }

    public LocationTimeZone TimeZone { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public IReadOnlyList<DepartmentLocation> DepartmentsLocations => _departmentsLocations;

    public IReadOnlyList<Department> Departments =>
        _departmentsLocations.Select(dl => dl.Department).ToList()!;

    private Location(
        Guid? id,
        LocationName name,
        Address address,
        LocationTimeZone timeZone)
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


    public static Location Create(
        LocationName name, Address address,
        LocationTimeZone timeZone)
    {
        var location = new Location(null, name, address, timeZone);

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