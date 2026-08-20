using Core;

namespace DirectoryService.Application.Departments.Queries;

public record GetByIdDepartmentQuery(Guid id) : IQuery;