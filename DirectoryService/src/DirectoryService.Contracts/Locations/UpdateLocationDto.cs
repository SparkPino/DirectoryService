namespace DirectoryService.Contracts.Locations;

public sealed record UpdateLocationDto(
    string? LocationName,
    LocationAdressDto? AdressDto,
    string? TimeZone);