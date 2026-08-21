using Core;

namespace DirectoryService.Application.Departments.Queries.SearchDepartmentTree;

public record SearchDepartmentTreeQuery(string Q) : IQuery;
