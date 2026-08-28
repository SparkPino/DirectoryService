namespace DirectoryService.Contracts.Locations;

public sealed class GetAllLocationDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }

    public AddressDto Address { get; init; }

    public DateTimeOffset CreatedAt { get; init; }

    public int AttachDepartmentCount { get; init; }
}
