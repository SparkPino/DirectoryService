using Core;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Departments.Queries.GetDepartments;

public record GetDepartmentsQuery : IQuery
{
    public string? Search { get; set; }

    public string? SortBy { get; set; }

    public SortDirection? SortDir { get; set; } = SortDirection.ASC;

    public Pagination? Pagination { get; set; }

    public bool? IsActive { get; set; }

    public Guid? ParentId { get; set; }

    public List<Guid>? LocationIds { get; set; }

    public List<Guid>? ExcludeIds { get; set; }
}