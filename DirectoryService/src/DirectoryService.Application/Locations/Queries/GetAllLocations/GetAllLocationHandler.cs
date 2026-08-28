using System.Linq.Expressions;
using System.Text.Json;
using Core;
using Core.Validations;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction.Repositories;
using DirectoryService.Contracts.Locations;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Application.Locations.Queries.GetAllLocations;

public class GetAllLocationHandler(
    ILocationReadRepository locationReadRepository,
    ILogger<GetAllLocationHandler> logger,
    IValidator<GetAllLocationQuery> validator)
    : IQueryHandler<GetAllLocationQuery, PagedResult<GetAllLocationDto>>
{
    private static readonly JsonSerializerOptions _addressJsonOptions = new() { PropertyNameCaseInsensitive = true, };

    private readonly ILocationReadRepository _locationReadRepository = locationReadRepository;
    private readonly ILogger<GetAllLocationHandler> _logger = logger;
    private readonly IValidator<GetAllLocationQuery> _validator = validator;

    public async Task<Result<PagedResult<GetAllLocationDto>, Errors>> Handle(
        GetAllLocationQuery query,
        CancellationToken cancellationToken)
    {
        var validatorResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validatorResult.IsValid)
        {
            return validatorResult.ToError();
        }

        IQueryable<LocationRow> locations = _locationReadRepository.SearchLocations(query.Search);

        if (query.MinDepartmentCount.HasValue && query.MinDepartmentCount.Value > 0)
        {
            locations = locations.Where(a => a.AttachDepartmentCount >= query.MinDepartmentCount.Value);
        }

        bool descending = query.SortDirection == SortDirection.DESC;

        if (query.OrderBy is null)
        {
            locations = descending
                ? locations.OrderByDescending(a => a.Name).ThenByDescending(a => a.Id)
                : locations.OrderBy(a => a.Name).ThenBy(a => a.Id);
        }
        else
        {
            Expression<Func<LocationRow, object>> selectorKey = query?.OrderBy?.Trim().ToLowerInvariant() switch
            {
                "name" => a => a.Name,
                "createdat" => a => a.CreatedAt,
                "departmentcount" => a => a.AttachDepartmentCount,
                _ => a => a.Name,
            };

            locations = descending
                ? locations.OrderByDescending(selectorKey).ThenByDescending(a => a.Id)
                : locations.OrderBy(selectorKey).ThenBy(a => a.Id);
        }


        int count = await locations.CountAsync(cancellationToken);

        int defaultPageSize = 20;
        int defaultPage = 1;

        int page = query.Page ?? defaultPage;
        int pageSize = query.PageSize ?? defaultPageSize;

        locations = locations
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        var rows = await locations.ToListAsync(cancellationToken);

        var items = rows
            .Select(r => new GetAllLocationDto()
            {
                Id = r.Id,
                Name = r.Name,
                CreatedAt = r.CreatedAt,
                Address = JsonSerializer.Deserialize<AddressDto>(r.Addresses, _addressJsonOptions)!,
                AttachDepartmentCount = r.AttachDepartmentCount,
            })
            .ToList();

        return new PagedResult<GetAllLocationDto>(items, count);
    }
}