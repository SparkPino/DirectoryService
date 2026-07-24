using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Abstraction.Database;
using DirectoryService.Application.Departments.Failures;
using DirectoryService.Application.Validations;
using DirectoryService.Contracts.Department;
using FluentValidation;
using Shared;

namespace DirectoryService.Application.Departments.Queries.GetDepartmentAncestors;

public class GetDepartmentAncestorsHandler(
    IValidator<GetDepartmentAncestorsQuery> validator,
    IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetDepartmentAncestorsQuery, List<DepartmentTreeNodeDto>>
{
    private readonly IValidator<GetDepartmentAncestorsQuery> _validator = validator;
    private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;

    public async Task<Result<List<DepartmentTreeNodeDto>, Errors>> Handle(
        GetDepartmentAncestorsQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToError();
        }

        const string sqlQuery = """
                                SELECT EXISTS(SELECT 1 FROM departments WHERE id = @DepartmentId AND is_active = true);

                                WITH child AS (SELECT d.id, d.path
                                               FROM departments d
                                               WHERE d.id = @DepartmentId)


                                SELECT d.id,
                                       d.name,
                                       d.identifier,
                                       d.path,
                                       d.depth,
                                       x.ChildrenCount > 0 AS HasChildren,
                                       x.ChildrenCount
                                FROM departments d
                                         CROSS JOIN child
                                         CROSS JOIN LATERAL (SELECT  COUNT(*) AS ChildrenCount FROM departments c WHERE c.parent_id = d.id AND c.is_active = true) x
                                WHERE d.path @> child.path
                                  AND d.id != child.id
                                AND d.is_active = true
                                ORDER BY d.depth;
                                """;
        var command = new CommandDefinition(sqlQuery, new { query.DepartmentId }, cancellationToken: cancellationToken);

        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        await using var multi = await connection.QueryMultipleAsync(command);

        bool exist = await multi.ReadFirstAsync<bool>();
        if (!exist)
        {
            return DepartmentError.NotFound(query.DepartmentId)
                .ToErrors();
        }

        var ancestors = (await multi.ReadAsync<DepartmentTreeNodeDto>()).ToList();

        return ancestors;
    }
}