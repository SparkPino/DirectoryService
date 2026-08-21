using Core;
using Core.Database;
using Core.Validations;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Contracts.Department;
using FluentValidation;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Departments.Queries.GetDirectChildrenDepartment;

public class GetDirectChildrenDepartmentHandler(
    IValidator<GetDirectChildrenDepartmentQuery> validator,
    IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetDirectChildrenDepartmentQuery, GetDirectChildrenDepartmentDto>
{
    private readonly IValidator<GetDirectChildrenDepartmentQuery> _validator = validator;
    private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;

    public async Task<Result<GetDirectChildrenDepartmentDto, Errors>> Handle(
        GetDirectChildrenDepartmentQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToError();
        }

        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        const string sqlQuery = """
                                 WITH current_level AS (SELECT d.id,
                                                       d.name,
                                                       d.identifier,
                                                       d.path,
                                                       d.depth,
                                                       d.parent_id,
                                                       d.is_active,
                                                       d.created_at,
                                                       d.updated_at,
                                                       d.deleted_at
                                                FROM departments d
                                               WHERE (d.parent_id = @parentId 
                                 OR (@parentId IS NULL AND d.parent_id IS NULL)) AND d.is_active = true
                                                ORDER BY d.created_at
                                                OFFSET @level_offset LIMIT @level_limit)

                                 SELECT *,
                                        (EXISTS(SELECT 1
                                                FROM departments d
                                                WHERE d.parent_id = current_level.id
                                                OFFSET @child_limit
                                                LIMIT 1)) AS has_more_children
                                 FROM current_level

                                 UNION ALL

                                 SELECT c.*,
                                        (EXISTS(SELECT 1 FROM departments WHERE parent_id = c.id)) AS has_more_children
                                 FROM current_level cl
                                          CROSS JOIN LATERAL (SELECT d.id,
                                                                     d.name,
                                                                     d.identifier,
                                                                     d.path,
                                                                     d.depth,
                                                                     d.parent_id,
                                                                     d.is_active,
                                                                     d.created_at,
                                                                     d.updated_at,
                                                                     d.deleted_at
                                                              FROM departments d
                                                              WHERE d.parent_id = cl.id
                                                                AND d.is_active = true
                                                              ORDER BY d.created_at
                                                              LIMIT @child_limit) c;
                                 """;

        var command = new CommandDefinition(
            sqlQuery,
            new { query.child_limit, query.level_limit, query.level_offset, query.ParentId },
            cancellationToken: cancellationToken);

        var rows = (await connection.QueryAsync<DirectChildDepartmentDto>(command)).ToList();

        var currentLevel = rows
            .Where(row => row.ParentId == query.ParentId)
            .Select(node => new DepartmentNodeDto
            {
                Id = node.Id,
                ParentId = node.ParentId,
                Name = node.Name,
                Identifier = node.Identifier,
                Path = node.Path,
                Depth = node.Depth,
                IsActive = node.IsActive,
                CreatedAt = node.CreatedAt,
                UpdatedAt = node.UpdatedAt,
                DeletedAt = node.DeletedAt,
                HasMoreChildren = node.HasMoreChildren,
                Children = rows.Where(child => child.ParentId == node.Id).ToList(),
            })
            .ToList();

        return new GetDirectChildrenDepartmentDto { Nodes = currentLevel };
    }
}