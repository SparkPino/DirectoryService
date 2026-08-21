using Core;

namespace DirectoryService.Application.Departments.Queries.GetDirectChildrenDepartment;

public record GetDirectChildrenDepartmentQuery(Guid? ParentId, int level_offset, int level_limit, int child_limit)
    : IQuery;