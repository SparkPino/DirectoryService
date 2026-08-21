using Core;
using Core.Database;
using Core.Validations;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Contracts.Department;
using FluentValidation;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Departments.Queries.GetDepartmentTree;

public class GetDepartmentTreeHandler(
    IValidator<GetDepartmentTreeQuery> validator,
    IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetDepartmentTreeQuery, List<DepartmentTreeNodeDto>>
{
    private readonly IValidator<GetDepartmentTreeQuery> _validator = validator;
    private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;

    public async Task<Result<List<DepartmentTreeNodeDto>, Errors>> Handle(
        GetDepartmentTreeQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToError();
        }

        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        const string sqlQuery = """
                                SELECT d.id,
                                       d.name,
                                       d.identifier,
                                       d.path,
                                       d.depth,
                                       x.ChildrenCount > 0 AS HasChildren,
                                       x.ChildrenCount
                                FROM departments d
                                         CROSS JOIN LATERAL (SELECT  COUNT(*) AS ChildrenCount FROM departments c WHERE c.parent_id = d.id AND c.is_active = true) x
                                WHERE d.parent_id IS NULL 
                                  AND d.is_active = true
                                ORDER BY d.created_at
                                LIMIT @Limit OFFSET @Offset;
                                """;

        var command = new CommandDefinition(sqlQuery, new { Limit = query.Limit ?? 50, Offset = query.Offset ?? 0 },
            cancellationToken: cancellationToken);

        var result = (await connection.QueryAsync<DepartmentTreeNodeDto>(command)).ToList();

        return result;
    }
}