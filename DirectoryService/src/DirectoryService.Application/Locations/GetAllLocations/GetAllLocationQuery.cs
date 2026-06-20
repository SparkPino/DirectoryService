using DirectoryService.Application.Abstraction;
using DirectoryService.Contracts.Locations;

namespace DirectoryService.Application.Locations.GetAllLocations;

public record GetAllLocationQuery(int Page, int PageSize) : IQuery<IReadOnlyList<AddLocationDto>>;