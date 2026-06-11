using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Locations.AddLocation;
using DirectoryService.Application.Locations.Failures;
using DirectoryService.Application.Locations.GetByIdLocation;
using DirectoryService.Application.Validations;
using DirectoryService.Domain.Locations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Application.Locations;

public class GetByIdLocationHandler : ICommandHandler<GetByIdLocationCommand, Location>
{
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<GetByIdLocationHandler> _logger;
    private readonly IValidator<GetByIdLocationCommand> _validator;

    public GetByIdLocationHandler(
        ILocationRepository locationRepository,
        ILogger<GetByIdLocationHandler> logger,
        IValidator<GetByIdLocationCommand> validator)
    {
        _locationRepository = locationRepository;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<Location, Errors>> Handle(
        GetByIdLocationCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToError();
        }

        var (_, isFailure, location, error) =
            await _locationRepository.GetByIdAsync(command.LocationId.Id, cancellationToken);

        if (isFailure)
        {
            return error.ToErrors();
        }

        return location;
    }
}