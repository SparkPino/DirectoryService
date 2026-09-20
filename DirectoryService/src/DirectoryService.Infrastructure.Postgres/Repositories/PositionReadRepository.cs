using DirectoryService.Application.Abstraction.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

public class PositionReadRepository : IPositionReadRepository
{
    private readonly DirectoryServiceDbContext _context;

    public PositionReadRepository(DirectoryServiceDbContext context)
    {
        _context = context;
    }

    public IQueryable<PositionRow> SearchPositions(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return _context.Database.SqlQuery<PositionRow>(
                $"""
                 SELECT
                     p.id AS "Id",
                     p.name AS "Name",
                     p.description AS "Description",
                     p.created_at AS "CreatedAt",
                     COUNT(dp.department_id) AS "AttachDepartmentCount"
                 FROM positions p
                 LEFT JOIN department_positions dp ON p.id = dp.position_id
                 WHERE is_active = TRUE
                 GROUP BY p.id, p.name, p.description, p.created_at
                 """);
        }

        string pattern = "%" + search.Trim() + "%";

        return _context.Database.SqlQuery<PositionRow>(
            $"""
             SELECT
                p.id AS "Id",
                p.name AS "Name",
                p.description AS "Description",
                p.created_at AS "CreatedAt",
                COUNT(dp.department_id) AS "AttachDepartmentCount"
             FROM positions p
             LEFT JOIN department_positions dp ON p.id = dp.position_id
             WHERE is_active = TRUE AND name ILIKE {pattern}
             GROUP BY p.id, p.name, p.description, p.created_at
             """);
    }
}
