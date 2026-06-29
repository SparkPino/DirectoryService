using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Departments;
using DirectoryService.Application.Departments.Commands.AddDepartment;
using DirectoryService.Application.Departments.Commands.AttachLocationToDepartment;
using DirectoryService.Application.Departments.Commands.AttachPositionToDepartment;
using DirectoryService.Application.Departments.Commands.DetachLocationFromDepartment;
using DirectoryService.Application.Departments.Commands.DetachPositionFromDepartment;
using DirectoryService.Application.Departments.Commands.MoveDepartment;
using DirectoryService.Application.Departments.Commands.RemoveDepartment;
using DirectoryService.Application.Departments.Commands.UpdateDepartment;
using DirectoryService.Application.Departments.Queries;
using DirectoryService.Contracts.Department;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Presenters.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.EndpointResult;

namespace DirectoryService.Presenters.Department;

[ApiController]
[Route("/api/departments/")]
[Produces("application/json")]
public class DepartmentController : BaseApiController
{
    [HttpGet("{departmentId:guid}/Dapper")]
    public async Task<EndpointResult<DepartmentRow>> GetByIdDapper(
        [FromRoute] Guid departmentId,
        [FromServices] IQueryHandler<GetByIdDepartmentQuery, DepartmentRow> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(new GetByIdDepartmentQuery(departmentId), cancellationToken);

    [ProducesResponseType(typeof(Envelope<Guid>), StatusCodes.Status200OK)]
    [HttpPost]
    public async Task<EndpointResult<Guid>> Create(
        [FromBody] DepartmentDto departmentDto,
        [FromServices] ICommandHandler<AddDepartmentCommand, Guid> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(new AddDepartmentCommand(departmentDto), cancellationToken);

    [HttpGet("{departmentId:guid}")]
    public async Task<EndpointResult<DepartmentDto>> GetById(
        [FromRoute] Guid departmentId,
        [FromServices] IQueryHandler<GetByIdDepartmentQuery, DepartmentDto> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(new GetByIdDepartmentQuery(departmentId), cancellationToken);

    [HttpDelete]
    public async Task<EndpointResult<Unit>> Delete(
        [FromBody] Guid departmentId,
        [FromServices] ICommandHandler<RemoveDepartmentCommand, Unit> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(new RemoveDepartmentCommand(departmentId), cancellationToken);

    [HttpPatch]
    [Route("/api/departments/{departmentId}")]
    public async Task<EndpointResult<Guid>> Update(
        [FromBody] UpdateDepartmentDto departmentDto,
        [FromRoute] Guid departmentId,
        [FromServices] ICommandHandler<UpdateDepartmentCommand, Guid> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(new UpdateDepartmentCommand(departmentDto, departmentId), cancellationToken);

    [HttpPost]
    [Route("/api/departments/{departmentId}/locations/{locationId}")]
    public async Task<EndpointResult<Guid>> AttachLocation(
        [FromRoute] Guid departmentId,
        [FromRoute] Guid locationId,
        [FromServices] ICommandHandler<AttachLocationToDepartmentCommand, Guid> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(new AttachLocationToDepartmentCommand(departmentId, locationId), cancellationToken);


    [HttpDelete]
    [Route("/api/departments/{departmentId}/locations/{locationId}")]
    public async Task<EndpointResult<Guid>> DetachLocation(
        [FromRoute] Guid departmentId,
        [FromRoute] Guid locationId,
        [FromServices] ICommandHandler<DetachLocationFromDepartmentCommand, Guid> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(new DetachLocationFromDepartmentCommand(departmentId, locationId), cancellationToken);

    [HttpPatch]
    [Route("/api/departments/{departmentId}/move")]
    public async Task<EndpointResult<Guid>> Move(
        [FromRoute] Guid departmentId,
        [FromBody] MoveDepartmentDto moveDepartmentDto,
        [FromServices] ICommandHandler<MoveDepartmentCommand, Guid> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(new MoveDepartmentCommand(departmentId, moveDepartmentDto.NewParentId), cancellationToken);

    [HttpPost]
    [Route("/api/departments/{departmentId}/positions/{positionId}")]
    public async Task<EndpointResult<Guid>> AttachPosition(
        [FromRoute] Guid departmentId,
        [FromRoute] Guid positionId,
        [FromServices] ICommandHandler<AttachPositionToDepartmentCommand, Guid> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(new AttachPositionToDepartmentCommand(departmentId, positionId), cancellationToken);

    [HttpDelete]
    [Route("/api/departments/{departmentId}/positions/{positionId}")]
    public async Task<EndpointResult<Guid>> DetachPosition(
        [FromRoute] Guid departmentId,
        [FromRoute] Guid positionId,
        [FromServices] ICommandHandler<DetachPositionFromDepartmentCommand, Guid> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(new DetachPositionFromDepartmentCommand(departmentId, positionId), cancellationToken);
}