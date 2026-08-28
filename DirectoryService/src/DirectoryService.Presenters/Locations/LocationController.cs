using Core;
using DirectoryService.Application.Locations.Commands.AddLocation;
using DirectoryService.Application.Locations.Commands.RemoveLocation;
using DirectoryService.Application.Locations.Commands.SoftDeleteLocation;
using DirectoryService.Application.Locations.Commands.UpdateLocation;
using DirectoryService.Application.Locations.Queries.GetAllLocations;
using DirectoryService.Application.Locations.Queries.GetByIdLocation;
using DirectoryService.Application.Locations.Queries.GetLocationTop;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Presenters.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.EndpointResult;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Presenters.Locations;

[ApiController]
[Route("/api/locations")]
[Produces("application/json")]
public class LocationController : BaseApiController
{
    [HttpGet("top")]
    public async Task<EndpointResult<IReadOnlyCollection<GetLocationTopDto>>> GetTop(
        [FromServices] IQueryHandler<IReadOnlyCollection<GetLocationTopDto>> handler,
        CancellationToken cancellationToken)
        => await handler.Handle(cancellationToken);

    [HttpGet]
    public async Task<EndpointResult<PagedResult<GetAllLocationDto>>> GetAll(
        [FromQuery] GetAllLocationQuery query,
        [FromServices] IQueryHandler<GetAllLocationQuery, PagedResult<GetAllLocationDto>> handler,
        CancellationToken cancellationToken) => await handler.Handle(query, cancellationToken);

    [HttpGet("dapper")]
    public async Task<EndpointResult<PagedResult<GetAllLocationDto>>> GetAllDapper(
        [FromQuery] GetAllLocationQueryDapper query,
        [FromServices] IQueryHandler<GetAllLocationQueryDapper, PagedResult<GetAllLocationDto>> handler,
        CancellationToken cancellationToken) => await handler.Handle(query, cancellationToken);

    [HttpGet("{locationId:guid}")]
    public async Task<EndpointResult<LocationResponseDto>> GetById(
        [FromRoute] Guid locationId,
        [FromServices] IQueryHandler<GetByIdLocationQuery, LocationResponseDto> handler,
        CancellationToken cancellationToken)
    {
        return await handler.Handle(new GetByIdLocationQuery(new LocationId(locationId)), cancellationToken);
    }

    [ProducesResponseType(typeof(Envelope<Guid>), StatusCodes.Status200OK)]
    [HttpPost]
    public async Task<EndpointResult<Guid>> Create(
        [FromBody] AddLocationDto addLocationDto,
        [FromServices] ICommandHandler<AddLocationCommand, Guid> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(new AddLocationCommand(addLocationDto), cancellationToken);

    [HttpPatch("{id:guid}")]
    public async Task<EndpointResult<Guid>> UpdateById(
        [FromBody] UpdateLocationDto locationDto,
        [FromRoute] Guid id,
        [FromServices] ICommandHandler<UpdateLocationCommand, Guid> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(new UpdateLocationCommand(locationDto, id), cancellationToken);

    [HttpDelete("{id:guid}")]
    public async Task<EndpointResult<Guid>> Delete(
        [FromRoute] Guid id,
        [FromServices] ICommandHandler<RemoveLocationCommand, Guid> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(new RemoveLocationCommand(id), cancellationToken);

    [HttpDelete("{id:guid}/soft-delete")]
    public async Task<EndpointResult<Guid>> SoftDelete(
        [FromRoute] Guid id,
        [FromServices] ICommandHandler<SoftDeleteLocationCommand, Guid> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(new SoftDeleteLocationCommand(new LocationId(id)), cancellationToken);
}