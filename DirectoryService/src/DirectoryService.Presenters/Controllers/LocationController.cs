using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Locations;
using DirectoryService.Application.Locations.AddLocation;
using DirectoryService.Contracts;
using DirectoryService.Domain.Locations;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presenters.Controllers;

[ApiController]
[Route("/api/locations")]
public class LocationController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] LocationDto locationDto,
        [FromServices] ICommandHandler<AddLocationCommand> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(new AddLocationCommand(locationDto), cancellationToken);

        if (result.IsFailure) return BadRequest(result.Error);

        return Ok();
    }
}