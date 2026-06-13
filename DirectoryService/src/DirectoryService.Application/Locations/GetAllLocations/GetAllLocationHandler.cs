using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Validations;
using DirectoryService.Contracts.Locations;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Locations.GetAllLocations;

public class GetAllLocationHandler(
    ILocationRepository locationRepository,
    ILogger<GetAllLocationHandler> logger,
    IValidator<GetAllLocationQuery> validator)
    : IQueryHandler<GetAllLocationQuery, IReadOnlyList<AddLocationDto>>
{
    private readonly ILocationRepository _locationRepository = locationRepository;
    private readonly ILogger<GetAllLocationHandler> _logger = logger;
    private readonly IValidator<GetAllLocationQuery> _validator = validator;

    public async Task<Result<IReadOnlyList<AddLocationDto>, Errors>> Handle(
        GetAllLocationQuery query,
        CancellationToken cancellationToken)
    {
        var validatorResult = await _validator.ValidateAsync(query);
        if (!validatorResult.IsValid)
        {
            return validatorResult.ToError();
        }

        var locations = await _locationRepository.GetPaged(query.Page, query.PageSize, cancellationToken);


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