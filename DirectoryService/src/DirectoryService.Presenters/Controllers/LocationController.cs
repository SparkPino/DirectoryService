using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Locations;
using DirectoryService.Application.Locations.AddLocation;
using DirectoryService.Contracts;
using DirectoryService.Domain.Locations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.EndpointResult;

namespace DirectoryService.Presenters.Controllers;

[ApiController]
[Route("/api/locations")]
[Produces("application/json")]
public class LocationController : BaseApiController
{
    [ProducesResponseType(typeof(Envelope<Guid>), StatusCodes.Status200OK)]
    [HttpPost]
    public async Task<EndpointResult<Guid>> Create(
        [FromBody] LocationDto locationDto,
        [FromServices] ICommandHandler<AddLocationCommand> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(new AddLocationCommand(locationDto), cancellationToken);
}