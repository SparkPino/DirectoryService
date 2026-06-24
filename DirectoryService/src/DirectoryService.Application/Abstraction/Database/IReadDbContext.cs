using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;

namespace DirectoryService.Application.Abstraction.Database;

public interface IReadDbContext
{
    IQueryable<Location> ReadLocations { get; }

    IQueryable<Department> ReadDepartments { get; }
}