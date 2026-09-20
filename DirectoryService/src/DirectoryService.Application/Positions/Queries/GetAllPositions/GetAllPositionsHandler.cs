using System.Linq.Expressions;
using Core;
using Core.Validations;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction.Repositories;
using DirectoryService.Contracts.Positions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Positions.Queries.GetAllPositions;

public class GetAllPositionsHandler(
    IPositionReadRepository positionReadRepository,
    ILogger<GetAllPositionsHandler> logger,
    IValidator<GetAllPositionsQuery> validator)
    : IQueryHandler<GetAllPositionsQuery, PagedResult<GetAllPositionDto>>
{
    private readonly IPositionReadRepository _positionReadRepository = positionReadRepository;
    private readonly ILogger<GetAllPositionsHandler> _logger = logger;
    private readonly IValidator<GetAllPositionsQuery> _validator = validator;

    public async Task<Result<PagedResult<GetAllPositionDto>, Errors>> Handle(
        GetAllPositionsQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToError();
        }

        _logger.LogInformation("GetAllPositionsHandler начал обработку запроса");

        IQueryable<PositionRow> positions = _positionReadRepository.SearchPositions(query.Search);

        bool descending = query.SortDir == SortDirection.DESC;

        Expression<Func<PositionRow, object>>? selectorKey = query.SortBy?.Trim().ToLowerInvariant() switch
        {
            "name" or null => a => a.Name,
            "created_at" => a => a.CreatedAt,
            _ => null,
        };

        if (selectorKey is null)
        {
            return Error.Validation(
                "get.positions.invalid.field",
                "Сортировать можно только по имени и дате создания",
                "SortBy").ToErrors();
        }

        positions = descending
            ? positions.OrderByDescending(selectorKey).ThenByDescending(a => a.Id)
            : positions.OrderBy(selectorKey).ThenBy(a => a.Id);

        int totalCount = await positions.CountAsync(cancellationToken);

        const int pageSizeDefault = 20;
        const int pageDefault = 1;

        int page = query.Pagination?.Page ?? pageDefault;
        int pageSize = query.Pagination?.PageSize ?? pageSizeDefault;
        int totalPage = (int)Math.Ceiling(totalCount / (float)pageSize);

        positions = positions
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        var rows = await positions.ToListAsync(cancellationToken);

        var items = rows
            .Select(r => new GetAllPositionDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                CreatedAt = r.CreatedAt,
                AttachDepartmentCount = r.AttachDepartmentCount,
            })
            .ToList();

        return new PagedResult<GetAllPositionDto>(items, totalCount, page, pageSize, totalPage);
    }
}
