using CSharpFunctionalExtensions;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Shared.ValueObjects;
using Shared;

namespace DirectoryService.Domain.Locations;

public sealed class Location
{
    private readonly List<DepartmentLocation> _departmentsLocations = [];

    public LocationId Id { get; private set; } = null!;

    public LocationName Name { get; private set; } = null!;

    public Address Address { get; private set; } = null!;

    public LocationTimeZone TimeZone { get; private set; } = null!;

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public IReadOnlyList<DepartmentLocation> DepartmentsLocations => _departmentsLocations;

    private Location(
        LocationId? id,
        LocationName name,
        Address address,
        LocationTimeZone timeZone)
    {
        Id = id ?? new LocationId(Guid.NewGuid());
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
        LocationTimeZone timeZone, LocationId? positionId = null)
    {
        var location = new Location(positionId, name, address, timeZone);

        return location;
    }

    public UnitResult<Error> AddDepartment(DepartmentId departmentId)
    {
        if (_departmentsLocations.Any(d => d.DepartmentId == departmentId))
            return Error.Conflict("department.already.exist", "Департамент уже добавлен");

        var departmentLocation = new DepartmentLocation(departmentId, Id);

        _departmentsLocations.Add(departmentLocation);
        UpdatedAt = DateTimeOffset.UtcNow;

        return UnitResult.Success<Error>();
    }
}