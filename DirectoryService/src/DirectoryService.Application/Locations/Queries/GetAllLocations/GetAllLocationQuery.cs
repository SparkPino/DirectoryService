using Core;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Locations.Queries.GetAllLocations;

public record GetAllLocationQuery(
    List<Guid>? DepartmentIds,
    string? Search,
    int? MinDepartmentCount,
    string? OrderBy,
    SortDirection? SortDirection,
    int? Page,
    int? PageSize)
    : IQuery;