using Core;
using Core.Database;
using Core.Validations;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Contracts.Department;
using FluentValidation;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Departments.Queries.SearchDepartmentTree;

public class SearchDepartmentTreeHandler(
    IValidator<SearchDepartmentTreeQuery> validator,
    IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<SearchDepartmentTreeQuery, List<DepartmentSearchResultDto>>
{
    private readonly IValidator<SearchDepartmentTreeQuery> _validator = validator;
    private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;

    public async Task<Result<List<DepartmentSearchResultDto>, Errors>> Handle(
        SearchDepartmentTreeQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToError();
        }

        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        const string sqlQuery = """
                                WITH node AS (
                                    SELECT d.id,
                                           d.name,
                                           d.identifier,
                                           d.path,
                                           d.depth,
                                           d.parent_id,
                                           d.id AS match_id
                                    FROM departments d
                                    WHERE d.is_active = true
                                      AND d.name ILIKE @Q
                                ORDER BY d.created_at)

                                SELECT *
                                FROM node

                                UNION ALL

                                SELECT c.*, nd.id AS match_id
                                FROM node nd
                                CROSS JOIN LATERAL ( SELECT
                                    d.id,
                                    d.name,
                                    d.identifier,
                                    d.path,
                                    d.depth,
                                    d.parent_id
                                FROM departments d
                                    WHERE d.path @> nd.path
                                      AND d.path != nd.path
                                    AND d.is_active = true
                                    ORDER BY d.created_at) c
                                """;

        var command = new CommandDefinition(sqlQuery, new { Q = $"%{query.Q}%" }, cancellationToken: cancellationToken);
        var departmentTreeNodeDtoList = (await connection.QueryAsync<DepartmentTreeNodeDto>(command)).ToList();

        var ancestorList = departmentTreeNodeDtoList
            .Where(a => a.Id != a.MatchId)
            .ToLookup(a => a.MatchId);
        var node = departmentTreeNodeDtoList.Where(a => a.Id == a.MatchId).ToList();
        var treeResult = node.Select(a => new DepartmentSearchResultDto()
        {
            Node = a,
            Ancestors = ancestorList[a.Id].ToList(),
        }).ToList();

        return treeResult;
    }
}