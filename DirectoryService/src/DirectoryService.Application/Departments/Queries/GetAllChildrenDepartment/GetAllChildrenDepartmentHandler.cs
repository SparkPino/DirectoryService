using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Abstraction.Database;
using DirectoryService.Application.Validations;
using DirectoryService.Contracts.Department;
using FluentValidation;
using Shared;

namespace DirectoryService.Application.Departments.Commands.GetAllChildren;

public class GetAllChildrenDepartmentHandler : IQueryHandler<GetAllChildrenQuery, GetAllDepartmentChildrenDto>
{
    private readonly IValidator<GetAllChildrenQuery> _validator;
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public GetAllChildrenDepartmentHandler(IValidator<GetAllChildrenQuery> validator,
        IDbConnectionFactory dbConnectionFactory)
    {
        _validator = validator;
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<Result<GetAllDepartmentChildrenDto, Errors>> Handle(
        GetAllChildrenQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToError();
        }

        const string recursiveSQL = """
                                    WITH RECURSIVE all_departments_family AS ( 
                                    SELECT  d.id, d.name,  d.path, d.identifier , 0 AS level, d.parent_id
                                                                               FROM departments d
                                                                               WHERE d.identifier = @rootIdentifier
                                                                               
                                                                               UNION ALL
                                                                               SELECT  c.id, c.name,  c.path, c.identifier , d.level + 1 ,c.parent_id
                                                                               FROM departments c
                                                                               JOIN all_departments_family d ON c.parent_id = d.id)


                                    SELECT * FROM all_departments_family
                                    ORDER BY level, id;
                                    """;

        const string materializePath = """
                                       WITH child AS (SELECT d.path  FROM departments d WHERE d.path = @rootIdentifier::ltree OR d.identifier = @rootIdentifier)
                                       SELECT *,nlevel(d.path) as Level
                                       FROM departments d
                                       CROSS JOIN child
                                       WHERE d.path <@ child.path
                                       ORDER BY d.depth
                                       """;

        const string giveMeSosedej = """
                                     SELECT * FROM departments d 
                                     WHERE  subpath(d.path,0 ,nlevel(d.path) -1) = subpath(@rootIdentifier::ltree,0,nlevel(@rootIdentifier::ltree) -1)
                                     AND d.path != @rootIdentifier::ltree
                                     ORDER BY d.id;
                                     """;

        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        var command = new CommandDefinition(materializePath, new { query.rootIdentifier },
            cancellationToken: cancellationToken);

        var queryResult = (await
            connection.QueryAsync<GetAllDepartmentChildrenDto>(command)).ToList();

        if (queryResult.Count == 0) return Error.NotFound("root.not.found").ToErrors();

        var dictionaryDepartmentChildrenDtos = queryResult.ToDictionary(a => a.Id);

        GetAllDepartmentChildrenDto root = new();
        foreach (var row in queryResult)
        {
            if (row.ParentId.HasValue &&
                dictionaryDepartmentChildrenDtos.TryGetValue(row.ParentId.Value, out var parent))
            {
                parent.childrens.Add(dictionaryDepartmentChildrenDtos[row.Id]);
            }
            else
            {
                root = dictionaryDepartmentChildrenDtos[row.Id];
            }
        }

        return root;
    }
}