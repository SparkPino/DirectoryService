using DirectoryService.Application.Abstraction;

namespace DirectoryService.Application.Locations.GetAllLocations;

public record GetAllLocationQuery(int Page, int PageSize) : IQuery;