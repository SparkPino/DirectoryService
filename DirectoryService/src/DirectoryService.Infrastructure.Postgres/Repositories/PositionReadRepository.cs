using DirectoryService.Application.Abstraction.Repositories;
using DirectoryService.Domain.Departments.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

public class PositionReadRepository : IPositionReadRepository
{
    private readonly DirectoryServiceDbContext _context;

    public PositionReadRepository(DirectoryServiceDbContext context)
    {
        _context = context;
    }

    public IQueryable<PositionRow> SearchPositions(Guid? departmentId, string? search)
    {
        var query = _context.Database.SqlQuery<PositionRow>(
            $"""
             SELECT
                 p.id AS "Id",
                 p.name AS "Name",
                 p.description AS "Description",
                 p.created_at AS "CreatedAt",
                 COUNT(dp.department_id) AS "AttachDepartmentCount"
             FROM positions p
             LEFT JOIN department_positions dp ON p.id = dp.position_id
             WHERE p.is_active = TRUE
             GROUP BY p.id, p.name, p.description, p.created_at
             """);

        if (departmentId.HasValue)
        {
            var positionIds = _context.DepartmentPositions
                .Where(a => a.DepartmentId == new DepartmentId(departmentId.Value))
                .Select(a => a.PositionId.Id);

            query = query.Where(r => positionIds.Contains(r.Id));
        }


        if (!string.IsNullOrWhiteSpace(search))
        {
            string pattern = "%" + search.Trim() + "%";
            query = query.Where(r => EF.Functions.ILike(r.Name, pattern));
        }

        return query;
    }

    
}