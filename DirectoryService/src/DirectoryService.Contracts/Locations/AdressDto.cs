namespace DirectoryService.Contracts;

public record AdressDto
{
    public string Country { get; set; }

    public string City { get; set; }

    public string Street { get; set; }

    public string PostalCode { get; set; }

    public string BuildingNumber { get; set; }

    public string? Apartment { get; set; }
}