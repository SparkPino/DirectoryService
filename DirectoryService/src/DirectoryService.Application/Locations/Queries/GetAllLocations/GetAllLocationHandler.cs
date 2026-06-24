using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Abstraction.Database;
using DirectoryService.Application.Abstraction.Repositories;
using DirectoryService.Application.Validations;
using DirectoryService.Contracts.Locations;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Locations.Queries.GetAllLocations;

public class GetAllLocationHandler(
    IReadDbContext readDbContext,
    ILogger<GetAllLocationHandler> logger,
    IValidator<GetAllLocationQuery> validator)
    : IQueryHandler<GetAllLocationQuery, IReadOnlyList<AddLocationDto>>
{
    private readonly IReadDbContext _readDbContext = readDbContext;
    private readonly ILogger<GetAllLocationHandler> _logger = logger;
    private readonly IValidator<GetAllLocationQuery> _validator = validator;

    public async Task<Result<IReadOnlyList<AddLocationDto>, Errors>> Handle(
        GetAllLocationQuery query,
        CancellationToken cancellationToken)
    {
        var validatorResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validatorResult.IsValid)
        {
            return validatorResult.ToError();
        }

        var locations = _readDbContext.ReadLocations
            .OrderBy(l => l.Name).ThenBy(l => l.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize);

        await locations.ToListAsync(cancellationToken);

        var result = locations.Select(a =>
            new AddLocationDto
            {
                Name = a.Name.Name,
                TimeZone = a.TimeZone.TimeZone,
                Address = new AddressDto()
                {
                    Country = a.Address.Country,
                    City = a.Address.City,
                    Street = a.Address.Street,
                    BuildingNumber = a.Address.BuildingNumber,
                    PostalCode = a.Address.PostalCode,
                    Apartment = a.Address.Apartment,
                },
            }).ToList();

        return result;
    }
}