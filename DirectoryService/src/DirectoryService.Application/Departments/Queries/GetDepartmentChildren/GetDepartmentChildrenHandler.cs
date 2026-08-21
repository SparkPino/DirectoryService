using Core;
using Core.Database;
using Core.Validations;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Departments.Failures;
using DirectoryService.Contracts.Department;
using FluentValidation;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Departments.Queries.GetDepartmentChildren;

public class GetDepartmentChildrenHandler(
    IValidator<GetDepartmentChildrenQuery> validator,
    IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetDepartmentChildrenQuery, List<DepartmentTreeNodeDto>>
{
    private readonly IValidator<GetDepartmentChildrenQuery> _validator = validator;
    private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;

    public async Task<Result<List<DepartmentTreeNodeDto>, Errors>> Handle(
        GetDepartmentChildrenQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToError();
        }

        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT EXISTS(SELECT 1 FROM departments WHERE id = @DepartmentId AND is_active = true);

                                SELECT d.id,
                                       d.name,
                                       d.identifier,
                                       d.path,
                                       d.depth,
                                       x.ChildrenCount > 0 AS HasChildren,
                                       x.ChildrenCount
                                FROM departments d
                                         CROSS JOIN LATERAL (SELECT  COUNT(*) AS ChildrenCount
                                                             FROM departments c 
                                                             WHERE c.parent_id = d.id AND c.is_active = true) x
                                WHERE d.parent_id = @DepartmentId
                                  AND d.is_active = true
                                ORDER BY d.created_at;
                                """;
        var command = new CommandDefinition(sqlQuery, new { query.DepartmentId }, cancellationToken: cancellationToken);
        await using var multi = await connection.QueryMultipleAsync(command);

        bool exist = await multi.ReadFirstAsync<bool>();
        if (!exist)
        {
            return DepartmentError.NotFound(query.DepartmentId)
                .ToErrors();
        }

        var result = (await multi.ReadAsync<DepartmentTreeNodeDto>()).ToList();

        return result;
    }
}