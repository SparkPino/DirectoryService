using CSharpFunctionalExtensions;

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

    public static Result<Address, string> Create(
        string country,
        string city,
        string street,
        string postalCode,
        string buildingNumber,
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