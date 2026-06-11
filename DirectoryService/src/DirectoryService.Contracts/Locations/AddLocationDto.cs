namespace DirectoryService.Contracts.Locations;

public sealed record AddLocationDto
{
    public string Name { get; init; }

    public AddressDto Address { get; init; }

    public string TimeZone { get; init; }
}