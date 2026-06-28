using DirectoryService.Application.Abstraction;
using DirectoryService.Contracts.Locations;

namespace DirectoryService.Application.Locations.Queries.GetLocationTop;

public record GetLocationTopQuery : IQuery
{
    public Guid Id { get; init; }
}

public record GetLocationTopDto
{
    public string Name { get; init; }
    public LocationAdressDto Addresses { get; init; }
    public int departmentCount { get; init; }
}

public record GetLocationTopRow
{
    public string Name { get; init; }
    public string Addresses { get; init; }
    public int departmentCount { get; init; }
}