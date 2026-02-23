using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain;

public class Location
{
    private readonly List<DepartmentsLocation> _departments;

    public Guid Id { get; private set; }

    public LocationName Name { get; private set; }

    public Address Address { get; private set; } //json ?  ValueObject

    public LocationTimeZone TimeZone { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyList<DepartmentsLocation> Departments => _departments;

    private Location(
        Guid? id,
        LocationName name,
        Address address,
        LocationTimeZone timeZone,
        IEnumerable<DepartmentsLocation> departments)
    {
        Id = id ?? Guid.NewGuid();
        Name = name;
        Address = address;
        TimeZone = timeZone;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
        _departments = departments.ToList();
    }

    private Location()
    {
    }


    public Result<Location, string> Create(
        IEnumerable<DepartmentsLocation> departments,
        LocationName name, Address adress,
        LocationTimeZone timeZone)
    {
        return new Location(Guid.NewGuid(), name, adress, timeZone, departments);
    }
}
