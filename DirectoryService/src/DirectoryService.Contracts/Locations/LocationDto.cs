namespace DirectoryService.Contracts;

public record LocationDto
{
    public string Name { get; set; }

    public AdressDto Adress { get; set; }

    public string TimeZone { get; set; }
}