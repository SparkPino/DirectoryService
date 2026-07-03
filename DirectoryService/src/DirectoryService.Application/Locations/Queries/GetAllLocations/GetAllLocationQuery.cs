using DirectoryService.Application.Abstraction;
using Shared;

namespace DirectoryService.Application.Locations.Queries.GetAllLocations;

public record GetAllLocationQuery(
    string? Search,
    int? minDepartmentCount,
    string? OrderBy,
    SortDirection? SortDirection,
    int? Page,
    int? PageSize)
    : IQuery;