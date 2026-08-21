using Core;

namespace DirectoryService.Application.Departments.Queries.GetDepartmentChildren;

public record GetDepartmentChildrenQuery(Guid DepartmentId) : IQuery;
