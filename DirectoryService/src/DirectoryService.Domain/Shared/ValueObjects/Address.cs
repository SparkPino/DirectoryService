using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.Shared.ValueObjects;

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

    public static Result<Address, Error> Create(
        string country,
        string city,
        string street,
        string postalCode,
        string buildingNumber,
        string? apartment)
    {
        if (string.IsNullOrWhiteSpace(country))
            return Error.Validation("address", "Страна не может быть пустой", nameof(Country));

        if (string.IsNullOrWhiteSpace(city))
            return Error.Validation("address", "Город не может быть пустой", nameof(City));

        if (string.IsNullOrWhiteSpace(street))
            return Error.Validation("address", "Улица не может быть пустой", nameof(Street));

        if (string.IsNullOrWhiteSpace(buildingNumber))
            return Error.Validation("address", "Номер дома не может быть пустой", nameof(BuildingNumber));

        if (string.IsNullOrWhiteSpace(postalCode))
            return Error.Validation("address", "Почтовый код не может быть пустым", nameof(PostalCode));


        return new Address(
            country.Trim(),
            city.Trim(),
            street.Trim(),
            postalCode.Trim(),
            buildingNumber.Trim(),
            apartment?.Trim());
    }
}