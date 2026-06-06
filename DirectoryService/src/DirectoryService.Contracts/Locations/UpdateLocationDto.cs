namespace DirectoryService.Contracts.Locations;

public record UpdateLocationDto(
    string? LocationName,
    LocationAdressDto? AdressDto,
    string? TimeZone);