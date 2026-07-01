using DirectoryService.Application.Abstraction;
using DirectoryService.Contracts;
using DirectoryService.Contracts.Department;
using Shared;

namespace DirectoryService.Application.Departments.Queries.GetDepartments;

public record GetDepartmentsQuery : IQuery
{
    public string? Search { get; set; }

    public string? SortBy { get; set; }

    public SortDirection? SortDir { get; set; } = SortDirection.ASC;

    public Pagination? Pagination { get; set; }
}