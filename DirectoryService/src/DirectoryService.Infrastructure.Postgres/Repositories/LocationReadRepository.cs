using DirectoryService.Application.Abstraction.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

public class LocationReadRepository : ILocationReadRepository
{
    private readonly DirectoryServiceDbContext _context;

    public LocationReadRepository(DirectoryServiceDbContext context)
    {
        _context = context;
    }

    public IQueryable<LocationRow> SearchLocations(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return _context.Database.SqlQuery<LocationRow>(
                $"""
                 SELECT 
                     l.id AS "Id",
                     l.name AS "Name", 
                     l.addresses AS "Addresses",
                     l.created_at AS "CreatedAt",
                     COUNT(dl.department_id) AS "AttachDepartmentCount"
                 FROM locations l
                 LEFT JOIN departments_location dl ON l.id = dl.location_id
                 WHERE is_active = TRUE
                 GROUP BY  l.id,l.name ,l.addresses, l.created_at
                 """);
        }

        string pattern = "%" + search.Trim() + "%";

        return _context.Database.SqlQuery<LocationRow>(
            $"""
             SELECT 
                l.id AS "Id",
                l.name AS "Name", 
                l.addresses AS "Addresses",
                l.created_at AS "CreatedAt",
                 COUNT(dl.department_id) AS "AttachDepartmentCount"
             FROM locations l
             LEFT JOIN departments_location dl ON l.id = dl.location_id
             WHERE is_active = TRUE AND name ILIKE {pattern}
             GROUP BY  l.id,l.name ,l.addresses, l.created_at
             """);
    }
}