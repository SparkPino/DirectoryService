namespace DirectoryService.Contracts.Locations;

public sealed class GetAllLocationDto
{
    public string Name { get; init; }

    public AddressDto Address { get; init; }

    public DateTimeOffset CreatedAt { get; init; }

    public int AttachDepartmentCount { get; init; }
}
