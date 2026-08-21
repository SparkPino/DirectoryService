using Core;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Departments.Queries.GetDepartments;

public record GetDepartmentsQuery : IQuery
{
    public string? Search { get; set; }

    public string? SortBy { get; set; }

    public SortDirection? SortDir { get; set; } = SortDirection.ASC;

    public Pagination? Pagination { get; set; }
}