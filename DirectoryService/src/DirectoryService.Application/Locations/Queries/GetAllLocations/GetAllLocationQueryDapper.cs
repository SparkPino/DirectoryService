using DirectoryService.Application.Abstraction;
using Shared;

namespace DirectoryService.Application.Locations.Queries.GetAllLocations;

public record GetAllLocationQueryDapper(
    string? Search,
    int? MinDepartmentCount,
    string? OrderBy,
    SortDirection? SortDirection,
    int? Page,
    int? PageSize)
    : IQuery;