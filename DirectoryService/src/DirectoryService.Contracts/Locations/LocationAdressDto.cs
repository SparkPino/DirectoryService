namespace DirectoryService.Contracts.Locations;

public sealed record LocationAdressDto(
    string? Country,
    string? City,
    string? Street,
    string? PostalCode,
    string? BuildingNumber,
    string? Apartment);