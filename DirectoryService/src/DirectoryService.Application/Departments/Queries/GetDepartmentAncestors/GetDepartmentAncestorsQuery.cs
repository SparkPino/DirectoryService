using Core;

namespace DirectoryService.Application.Departments.Queries.GetDepartmentAncestors;

public record GetDepartmentAncestorsQuery(Guid DepartmentId) : IQuery;
