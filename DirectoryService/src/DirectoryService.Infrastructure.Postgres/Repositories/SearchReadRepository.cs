using DirectoryService.Application.Abstraction.Repositories;
using DirectoryService.Contracts.Search;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

public class SearchReadRepository : ISearchReadRepository
{
    private readonly DirectoryServiceDbContext _context;
    private readonly ILogger<SearchReadRepository> _logger;

    public SearchReadRepository(
        DirectoryServiceDbContext context,
        ILogger<SearchReadRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<SearchResultItemDto>> GeneralSearch(string query, int limit,
        CancellationToken cancellationToken)
    {
        var search = $"%{query}%";
        var prefix = $"{query}%";

        FormattableString sqlQuery = $"""
                                      (SELECT 'Department' AS "Type",
                                              id           AS "Id",
                                              name         AS "Title",
                                              path::text   AS "Subtitle"
                                       FROM departments d
                                       WHERE d.name ILIKE {search}
                                         AND d.deleted_at is NULL
                                       ORDER BY CASE WHEN d.name ILIKE {prefix} THEN 0 ELSE 1 END, d.name ASC
                                       LIMIT {limit})


                                      UNION ALL

                                      (SELECT 'Location'                          AS "Type",
                                              id                                 AS "Id",
                                              name                                AS "Title",
                                              concat_ws(', ',
                                                        l.addresses ->> 'Country',
                                                        l.addresses ->> 'City',
                                                        l.addresses ->> 'Street') AS "Subtitle"
                                       FROM locations l
                                       WHERE l.name ILIKE {search}
                                         AND l.deleted_at is NULL
                                       ORDER BY CASE WHEN l.name ILIKE {prefix} THEN 0 ELSE 1 END, l.name ASC
                                       LIMIT {limit})

                                      UNION ALL

                                      (SELECT 'Position'                   AS "Type",
                                              p.id                         AS  "Id",
                                              p.name                       AS "Title",
                                              (SELECT string_agg(d.name, ', ' ORDER BY d.name)
                                               FROM departments d
                                                        JOIN department_positions dp ON p.id = dp.position_id
                                               WHERE dp.department_id = d.id
                                                 AND d.deleted_at IS NULL) AS "Subtitle"
                                       FROM positions p
                                       WHERE p.name ILIKE {search}
                                         AND p.deleted_at IS NULL
                                       ORDER BY CASE WHEN p.name ILIKE {prefix} THEN 0 ELSE 1 END, p.name ASC
                                       LIMIT {limit}) 
                                              
                                      """;

        var result = await _context.Database.SqlQuery<SearchResultItemDto>(sqlQuery).ToListAsync(cancellationToken);
        return result;
    }
}