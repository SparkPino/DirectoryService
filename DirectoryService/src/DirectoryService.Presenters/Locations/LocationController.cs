using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Locations;
using DirectoryService.Application.Locations.AddLocation;
using DirectoryService.Application.Locations.Failures;
using DirectoryService.Application.Locations.GetByIdLocation;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Presenters.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.EndpointResult;

namespace DirectoryService.Presenters.Locations;

[ApiController]
[Route("/api/locations")]
[Produces("application/json")]
public class LocationController : BaseApiController
{
    [ProducesResponseType(typeof(Envelope<Guid>), StatusCodes.Status200OK)]
    [HttpPost]
    public async Task<EndpointResult<Guid>> Create(
        [FromBody] LocationDto locationDto,
        [FromServices] ICommandHandler<AddLocationCommand, Guid> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(new AddLocationCommand(locationDto), cancellationToken);

    [HttpGet("{locationId:guid}")]
    public async Task<EndpointResult<Location>> GetById(
        [FromRoute] Guid locationId,
        [FromServices] ICommandHandler<GetByIdLocationCommand, Location> handler,
        CancellationToken cancellationToken)
    {
        return await handler.Handle(new GetByIdLocationCommand(new LocationId(locationId)), cancellationToken);
    }
}