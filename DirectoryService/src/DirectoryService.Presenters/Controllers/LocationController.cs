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
    private readonly ICommandHandler<AddLocationCommand> _commandHandler;

    public LocationController(ICommandHandler<AddLocationCommand> commandHandler)
    {
        _commandHandler = commandHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] LocationDto locationDto,
        CancellationToken cancellationToken)
    {
        var result = await _commandHandler.Handle(new AddLocationCommand(locationDto), cancellationToken);

        if (result.IsFailure) return BadRequest(result.Error);

        return Ok();
    }
}