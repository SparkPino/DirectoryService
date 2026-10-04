using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Positions;

namespace DirectoryService.Application.Abstraction.Database;

public interface IReadDbContext
{
    IQueryable<Location> ReadLocations { get; }

    IQueryable<Department> ReadDepartments { get; }
    
    IQueryable<Position> ReadPositions { get; }
}