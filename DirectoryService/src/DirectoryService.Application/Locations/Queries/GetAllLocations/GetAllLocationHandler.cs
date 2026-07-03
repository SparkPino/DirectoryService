using System.Text.Json;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Abstraction.Repositories;
using DirectoryService.Application.Validations;
using DirectoryService.Contracts.Locations;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

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

        if (query.minDepartmentCount.HasValue && query.minDepartmentCount.Value > 0)
        {
            locations = locations.Where(a => a.AttachDepartmentCount >= query.minDepartmentCount.Value);
        }

        bool descending = query.SortDirection == SortDirection.DESC;

        if (!string.IsNullOrWhiteSpace(query.OrderBy))
        {
            locations = query.OrderBy.Trim().ToLowerInvariant() switch
            {
                "name" => descending
                    ? locations.OrderByDescending(a => a.Name).ThenByDescending(a => a.Id)
                    : locations.OrderBy(a => a.Name).ThenBy(a => a.Id),
                "createdat" => descending
                    ? locations.OrderByDescending(a => a.CreatedAt).ThenByDescending(a => a.Id)
                    : locations.OrderBy(a => a.CreatedAt).ThenBy(a => a.Id),
                "departmentcount" => descending
                    ? locations.OrderByDescending(a => a.AttachDepartmentCount).ThenBy(a => a.Id)
                    : locations.OrderBy(a => a.AttachDepartmentCount).ThenBy(a => a.Id),
                _ => locations.OrderBy(l => l.Name).ThenBy(l => l.Id),
            };
        }
        else
        {
            locations = descending
                ? locations.OrderByDescending(a => a.Name).ThenByDescending(a => a.Id)
                : locations.OrderBy(a => a.Name).ThenBy(a => a.Id);
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
                Name = r.Name,
                CreatedAt = r.CreatedAt,
                Address = JsonSerializer.Deserialize<AddressDto>(r.Addresses, _addressJsonOptions)!,
                AttachDepartmentCount = r.AttachDepartmentCount,
            })
            .ToList();

        return new PagedResult<GetAllLocationDto>(items, count);
    }
}