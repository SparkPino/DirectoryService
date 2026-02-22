using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain;

public class Location
{
    private readonly List<DepartmentsLocation> _departments;

    public Guid Id { get; private set; }

    public LocationName LocationName { get; private set; }

    public Address Adress { get; private set; } //json ?  ValueObject

    public LocationTimeZone LocationTimeZone { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyList<DepartmentsLocation> Departments => _departments;

    private Location(
        Guid? id,
        LocationName locationName,
        Address adress,
        LocationTimeZone locationTimeZone,
        IEnumerable<DepartmentsLocation> departments)
    {
        Id = id ?? Guid.NewGuid();
        LocationName = locationName;
        Adress = adress;
        LocationTimeZone = locationTimeZone;
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
        LocationTimeZone locationTimeZone)
    {
        return new Location(Guid.NewGuid(), name, adress, locationTimeZone, departments);
    }
}

public record Address
{
    public string Country { get; }

    public string City { get; }

    public string Street { get; }

    public string PostalCode { get; }

    public string BuildingNumber { get; }

    public string? Apartment { get; }

    private Address(
        string country,
        string city,
        string street,
        string postalCode,
        string buildingNumber,
        string? apartment)
    {
        Country = country;
        City = city;
        Street = street;
        PostalCode = postalCode;
        BuildingNumber = buildingNumber;
        Apartment = apartment;
    }

    public static Result<Address, string> Create(
        string country,
        string city,
        string street,
        string postalCode,
        string? buildingNumber,
        string? apartment)
    {
        if (string.IsNullOrWhiteSpace(country))
            return "Страна не может быть пустой";

        if (string.IsNullOrWhiteSpace(city))
            return "Город не может быть пустой";

        if (string.IsNullOrWhiteSpace(street))
            return "Улица не может быть пустой";

        if (string.IsNullOrWhiteSpace(postalCode))
            return "Почтовый код не может быть пустым";


        return new Address(
            country.Trim(),
            city.Trim(),
            street.Trim(),
            postalCode.Trim(),
            buildingNumber.Trim(),
            apartment?.Trim());
    }

    public string GetTimeZone()
    {
        return Country + "/" + City;
    }
}