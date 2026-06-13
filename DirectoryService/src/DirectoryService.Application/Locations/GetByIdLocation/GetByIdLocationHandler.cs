using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Validations;
using DirectoryService.Domain.Locations;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Locations.GetByIdLocation;

public class GetByIdLocationHandler : IQueryHandler<GetByIdLocationQuery, Location>
{
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<GetByIdLocationHandler> _logger;
    private readonly IValidator<GetByIdLocationQuery> _validator;

    public GetByIdLocationHandler(
        ILocationRepository locationRepository,
        ILogger<GetByIdLocationHandler> logger,
        IValidator<GetByIdLocationQuery> validator)
    {
        _locationRepository = locationRepository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<Location, Errors>> Handle(
        GetByIdLocationQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToError();
        }

        var (_, isFailure, location, error) =
            await _locationRepository.GetByIdAsync(query.LocationId.Id, cancellationToken);

        if (isFailure)
        {
            return error.ToErrors();
        }

        return location;
    }
}