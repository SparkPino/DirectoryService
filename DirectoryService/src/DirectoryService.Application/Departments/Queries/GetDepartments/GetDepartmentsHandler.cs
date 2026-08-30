using System.Data;
using System.Text;
using Core;
using Core.Database;
using Core.Validations;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Contracts.Department;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Departments.Queries.GetDepartments;

public class GetDepartmentsHandler : IQueryHandler<GetDepartmentsQuery, PagedResult<GetDepartmentDto>>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly ILogger<GetDepartmentsHandler> _logger;
    private readonly IValidator<GetDepartmentsQuery> _validator;

    public GetDepartmentsHandler(IDbConnectionFactory dbConnectionFactory, ILogger<GetDepartmentsHandler> logger,
        IValidator<GetDepartmentsQuery> validator)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<PagedResult<GetDepartmentDto>, Errors>> Handle(
        GetDepartmentsQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToError();
        }

        _logger.LogInformation("GetDepartmentsHandler начал обработку запроса");


        StringBuilder sb = new StringBuilder();
        sb.Append(@"
        SELECT 
        d.id AS department_id,
        d.path,
        d.xmin::text::bigint AS row_version,
        d.name,
        d.created_at,
         COUNT(*) OVER() AS total_count
        FROM departments d
        WHERE is_active = true"); // якорь, чтобы все условия шли через AND если нада заглушка WHERE 1=1 если нету подходящего условия.

        var parameters = new DynamicParameters();

        
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string normalize = query.Search.Trim();
            sb.Append(" AND d.name ILIKE @Name");
            parameters.Add("Name", $"%{normalize}%");
        }

        string? sortBy = "d.name";
        if (!string.IsNullOrWhiteSpace(query.SortBy))
        {
            sortBy = query.SortBy switch
            {
                "name" => "d.name",
                "created_at" => "d.created_at",
                _ => null,
            };

            if (sortBy is null)
            {
                return Error.Validation(
                    "get.departments.invalid.field",
                    "Сортировать можно только по имени и дате создания",
                    "SortBy").ToErrors();
            }
        }

        string descending = query.SortDir == SortDirection.DESC ? " DESC" : " ASC";
        sb.Append($" ORDER BY {sortBy} {descending}, d.id");

        const int pageSizeDefault = 20;
        const int pageDefault = 1;

        int pageFromQuery = query.Pagination?.Page ?? pageDefault;
        int pageSize = query.Pagination?.PageSize ?? pageSizeDefault;
        int page = (pageFromQuery - 1) * pageSize;
        
        sb.Append(" LIMIT @Limit OFFSET @Offset");
        parameters.Add("Limit", pageSize,
            DbType.Int32); // Просто щоб показати що таке є для таких ситуацій string? name = null;
        parameters.Add("Offset", page);

        using var connectionAsync = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        var departmentsResult = (await connectionAsync.QueryAsync<GetDepartmentDto>(
            new CommandDefinition(sb.ToString(), parameters, cancellationToken: cancellationToken))).ToList();
     

        var count = departmentsResult.FirstOrDefault()?.TotalCount ?? 0;
        var totalPage = (int)count / pageSize;
        var result = new PagedResult<GetDepartmentDto>(departmentsResult, count, pageFromQuery, pageSize, totalPage);
        return result;
    }
}