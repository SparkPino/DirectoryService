using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Locations;
using DirectoryService.Application.Locations.AddLocation;
using DirectoryService.Application.Locations.Failures;
using DirectoryService.Application.Locations.GetAllLocations;
using DirectoryService.Application.Locations.GetByIdLocation;
using DirectoryService.Application.Locations.UpdateLocation;
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
        [FromBody] AddLocationDto addLocationDto,
        [FromServices] ICommandHandler<AddLocationCommand, Guid> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(new AddLocationCommand(addLocationDto), cancellationToken);

    [HttpGet("{locationId:guid}")]
    public async Task<EndpointResult<Location>> GetById(
        [FromRoute] Guid locationId,
        [FromServices] IQueryHandler<GetByIdLocationQuery, Location> handler,
        CancellationToken cancellationToken)
    {
        return await handler.Handle(new GetByIdLocationQuery(new LocationId(locationId)), cancellationToken);
    }

    [HttpPatch]
    [Route("/api/locations/{id}")]
    public async Task<EndpointResult<Guid>> UpdateById(
        [FromBody] UpdateLocationDto locationDto,
        [FromRoute] Guid id,
        [FromServices] ICommandHandler<UpdateLocationCommand, Guid> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(new UpdateLocationCommand(locationDto, id), cancellationToken);

    [HttpGet]
    public async Task<EndpointResult<IReadOnlyList<AddLocationDto>>> GetAll(
        [FromQuery] GetAllLocationQuery query,
        [FromServices] IQueryHandler<GetAllLocationQuery, IReadOnlyList<AddLocationDto>> handler,
        CancellationToken cancellationToken) => await handler.Handle(query, cancellationToken);
}