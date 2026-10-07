using DirectoryService.Application.Abstraction.Repositories;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Departments.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

public class LocationReadRepository : ILocationReadRepository
{
    private readonly DirectoryServiceDbContext _context;

    public LocationReadRepository(DirectoryServiceDbContext context)
    {
        _context = context;
    }

    public IQueryable<LocationRow> SearchLocations(List<Guid>? departmentIds, string? search)
    {
        var query = _context.Database.SqlQuery<LocationRow>(
            $"""
             SELECT 
                 l.id AS "Id",
                 l.name AS "Name", 
                 l.addresses AS "Addresses",
                 l.created_at AS "CreatedAt",
                 l.timezone AS "TimeZone",
                 COUNT(dl.department_id) AS "AttachDepartmentCount"
             FROM locations l
             LEFT JOIN departments_location dl ON l.id = dl.location_id
             WHERE is_active = TRUE
             GROUP BY  l.id,l.name ,l.addresses, l.created_at
             """);


        if (departmentIds != null && departmentIds.Count > 0)
        {
            var converteDepartmentIds =  departmentIds.Select(l => new DepartmentId(l)).ToList();
            var locationIds = _context.DepartmentLocations
                .Where(dl => converteDepartmentIds
                    .Contains(dl.DepartmentId))
                .Select(dl => EF.Property<Guid>(dl, nameof(DepartmentLocation.LocationId)));
            query = query.Where(a => locationIds.Contains(a.Id));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            string pattern = "%" + search.Trim() + "%";
            query = query.Where(l => EF.Functions.ILike(l.Name, pattern));
        }


        return query;
    }
}