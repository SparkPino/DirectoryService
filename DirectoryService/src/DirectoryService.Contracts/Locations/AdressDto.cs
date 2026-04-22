namespace DirectoryService.Contracts;

public record AdressDto
{
    public string Country { get; init; }

    public string City { get; init; }

    public string Street { get; init; }

    public string PostalCode { get; init; }

    public string BuildingNumber { get; init; }

    public string? Apartment { get; init; }
}