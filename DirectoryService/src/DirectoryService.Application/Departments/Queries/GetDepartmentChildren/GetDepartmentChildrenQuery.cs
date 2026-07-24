using DirectoryService.Application.Abstraction;

namespace DirectoryService.Application.Departments.Queries.GetDepartmentChildren;

public record GetDepartmentChildrenQuery(Guid DepartmentId) : IQuery;
