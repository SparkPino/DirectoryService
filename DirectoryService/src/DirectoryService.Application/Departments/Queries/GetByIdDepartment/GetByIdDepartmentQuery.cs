using DirectoryService.Application.Abstraction;
using DirectoryService.Contracts.Department;

namespace DirectoryService.Application.Departments.Queries;

public record GetByIdDepartmentQuery(Guid id) : IQuery;