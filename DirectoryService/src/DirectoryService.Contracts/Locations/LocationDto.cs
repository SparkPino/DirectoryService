namespace DirectoryService.Contracts;

public record LocationDto
{
    public string Name { get; init; }

    public AdressDto Adress { get; init; }

    public string TimeZone { get; init; }
}