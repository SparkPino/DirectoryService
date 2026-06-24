using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Abstraction.Database;
using DirectoryService.Application.Abstraction.Repositories;
using DirectoryService.Application.Locations.Failures;
using DirectoryService.Application.Validations;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Locations.Queries.GetByIdLocation;

public class GetByIdLocationHandler : IQueryHandler<GetByIdLocationQuery, LocationResponseDto>
{
    private readonly IReadDbContext _dbContext;
    private readonly ILogger<GetByIdLocationHandler> _logger;
    private readonly IValidator<GetByIdLocationQuery> _validator;

    public GetByIdLocationHandler(
        IReadDbContext dbContext,
        ILogger<GetByIdLocationHandler> logger,
        IValidator<GetByIdLocationQuery> validator)
    {
        _dbContext = dbContext;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<LocationResponseDto?, Errors>> Handle(
        GetByIdLocationQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToError();
        }

        var location = await _dbContext.ReadLocations
            .Where(a => a.Id == query.LocationId)
            .Select(a => new LocationResponseDto()
            {
                Id = a.Id.Id,
                Name = a.Name.Name,
                Address = new AddressDto()
                {
                    Apartment = a.Address.Apartment,
                    City = a.Address.City,
                    Country = a.Address.Country,
                    PostalCode = a.Address.PostalCode,
                    Street = a.Address.Street,
                    BuildingNumber = a.Address.BuildingNumber,
                },
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt,
                TimeZone = a.TimeZone.TimeZone,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (location is null)
        {
            _logger.LogWarning("Локация с id:{LocationId} не найдена.", query.LocationId.Id);
            return LocationErrors.NotFound(query.LocationId.Id).ToErrors();
        }

        return location;
    }
}