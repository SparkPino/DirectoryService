namespace DirectoryService.Contracts.Locations;

public record LocationAdressDto(
    string? Country,
    string? City,
    string? Street,
    string? PostalCode,
    string? BuildingNumber,
    string? Apartment);