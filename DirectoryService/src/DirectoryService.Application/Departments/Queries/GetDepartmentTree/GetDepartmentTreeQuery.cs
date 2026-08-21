using Core;

namespace DirectoryService.Application.Departments.Queries.GetDepartmentTree;

public record GetDepartmentTreeQuery(int? Limit, int? Offset) : IQuery;