using Core;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Positions.Queries.GetAllPositions;

public record GetAllPositionsQuery : IQuery
{
    public string? Search { get; set; }

    public string? SortBy { get; set; }

    public SortDirection? SortDir { get; set; } = SortDirection.ASC;

    public Pagination? Pagination { get; set; }
}
