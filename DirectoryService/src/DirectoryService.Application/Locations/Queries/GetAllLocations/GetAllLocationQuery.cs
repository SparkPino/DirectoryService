using DirectoryService.Application.Abstraction;

namespace DirectoryService.Application.Locations.Queries.GetAllLocations;

public record GetAllLocationQuery(int Page, int PageSize) : IQuery;