using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Locations.AddLocation;
using DirectoryService.Application.Locations.Failures;
using DirectoryService.Application.Locations.GetByIdLocation;
using DirectoryService.Domain.Locations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Locations;

public class GetByIdLocationHandler : ICommandHandler<GetByIdLocationCommand, Location>
{
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<GetByIdLocationHandler> _logger;

    public GetByIdLocationHandler(ILocationRepository locationRepository, ILogger<GetByIdLocationHandler> logger)
    {
        _locationRepository = locationRepository;
        _logger = logger;
    }

    public async Task<Result<Location, Errors>> Handle(
        GetByIdLocationCommand locationCommand,
        CancellationToken cancellationToken)
    {
        var (_, isFailure, location, error) =
            await _locationRepository.GetByIdAsync(locationCommand.LocationId.Id, cancellationToken);

        if (isFailure)
        {
            return error.ToErrors();
        }

        return location;
    }
}