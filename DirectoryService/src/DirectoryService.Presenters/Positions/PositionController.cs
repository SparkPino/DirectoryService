using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Positions.AddPosition;
using DirectoryService.Application.Positions.RemovePosition;
using DirectoryService.Application.Positions.SoftDeletePosition;
using DirectoryService.Application.Positions.UpdatePosition;
using DirectoryService.Contracts.Positions;
using DirectoryService.Domain.Positions.ValueObjects;
using DirectoryService.Presenters.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.EndpointResult;

namespace DirectoryService.Presenters.Positions;

[ApiController]
[Route("/api/positions")]
[Produces("application/json")]
public class PositionController : BaseApiController
{
    [ProducesResponseType(typeof(Envelope<Guid>), StatusCodes.Status200OK)]
    [HttpPost]
    public async Task<EndpointResult<Guid>> Create(
        [FromBody] AddPositionDto addPositionDto,
        [FromServices] ICommandHandler<AddPositionCommand, Guid> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(new AddPositionCommand(addPositionDto), cancellationToken);

    [HttpPatch]
    [Route("/api/positions/{id}")]
    public async Task<EndpointResult<Guid>> UpdateById(
        [FromBody] UpdatePositionDto updatePositionDto,
        [FromRoute] Guid id,
        [FromServices] ICommandHandler<UpdatePositionCommand, Guid> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(new UpdatePositionCommand(updatePositionDto, id), cancellationToken);

    [HttpDelete]
    [Route("/api/positions/{id}")]
    public async Task<EndpointResult<Guid>> Delete(
        [FromRoute] Guid id,
        [FromServices] ICommandHandler<RemovePositionCommand, Guid> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(new RemovePositionCommand(id), cancellationToken);

    [HttpDelete]
    [Route("/api/positions/{id}/soft-delete")]
    public async Task<EndpointResult<Guid>> SoftDelete(
        [FromRoute] Guid id,
        [FromServices] ICommandHandler<SoftDeletePositionCommand, Guid> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(new SoftDeletePositionCommand(new PositionId(id)), cancellationToken);
}