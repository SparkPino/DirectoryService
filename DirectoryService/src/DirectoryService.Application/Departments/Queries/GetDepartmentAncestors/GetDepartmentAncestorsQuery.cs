using DirectoryService.Application.Abstraction;

namespace DirectoryService.Application.Departments.Queries.GetDepartmentAncestors;

public record GetDepartmentAncestorsQuery(Guid DepartmentId) : IQuery;
