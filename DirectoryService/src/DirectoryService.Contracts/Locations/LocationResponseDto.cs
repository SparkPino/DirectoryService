namespace DirectoryService.Contracts.Locations;

public sealed record LocationResponseDto
{
    public Guid Id { get; init; }

    public string Name { get; init; }

    public AddressDto Address { get; init; }

    public string TimeZone { get; init; } = null!;

    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? UpdatedAt { get; init; }
}

public sealed record ShortAdressDTO
{
    public Guid Id { get; init; }
    
}