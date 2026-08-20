using Core;
using DirectoryService.Application.Departments.Commands.AddDepartment;
using DirectoryService.Application.Departments.Commands.AttachLocationToDepartment;
using DirectoryService.Application.Departments.Commands.AttachPositionToDepartment;
using DirectoryService.Application.Departments.Commands.DetachLocationFromDepartment;
using DirectoryService.Application.Departments.Commands.DetachPositionFromDepartment;
using DirectoryService.Application.Departments.Commands.GetAllChildren;
using DirectoryService.Application.Departments.Commands.MoveDepartment;
using DirectoryService.Application.Departments.Queries.GetDirectChildrenDepartment;
using DirectoryService.Application.Departments.Commands.RemoveDepartment;
using DirectoryService.Application.Departments.Commands.SoftDeleteDepartment;
using DirectoryService.Application.Departments.Commands.UpdateDepartment;
using DirectoryService.Application.Departments.Commands.UpdateDepartmentParent;
using DirectoryService.Application.Departments.Queries;
using DirectoryService.Application.Departments.Queries.GetDepartmentAncestors;
using DirectoryService.Application.Departments.Queries.GetDepartmentChildren;
using DirectoryService.Application.Departments.Queries.GetDepartments;
using DirectoryService.Application.Departments.Queries.GetDepartmentTree;
using DirectoryService.Application.Departments.Queries.SearchDepartmentTree;
using DirectoryService.Contracts.Department;
using DirectoryService.Contracts.Department.UpdateDepartmentParent;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Presenters.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.EndpointResult;
using SharedLibrary.SharedKernel;

namespace DirectoryService.Presenters.Department;

[ApiController]
[Route("/api/departments/")]
[Produces("application/json")]
public class DepartmentController : BaseApiController
{
    [HttpGet]
    public async Task<EndpointResult<PagedResult<GetDepartmentDto>>> GetDepartments(
        [FromQuery] GetDepartmentsQuery query,
        [FromServices] IQueryHandler<GetDepartmentsQuery, PagedResult<GetDepartmentDto>> handler,
        CancellationToken cancellationToken) => await handler.Handle(query, cancellationToken);

    [HttpPost("tree/identifier")]
    public async Task<EndpointResult<GetAllDepartmentChildrenDto>> GetDepartmentTreeFromIdentifier(
        [FromBody] GetAllChildrenQuery rootIdentifier,
        [FromServices] IQueryHandler<GetAllChildrenQuery, GetAllDepartmentChildrenDto> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(rootIdentifier, cancellationToken);


    [HttpPost("direct-children")]
    public async Task<EndpointResult<GetDirectChildrenDepartmentDto>> GetDirectChildren(
        [FromBody] GetDirectChildrenDepartmentQuery query,
        [FromServices] IQueryHandler<GetDirectChildrenDepartmentQuery, GetDirectChildrenDepartmentDto> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(query, cancellationToken);

    [HttpGet("tree")]
    public async Task<EndpointResult<List<DepartmentTreeNodeDto>>> GetTree(
        [FromQuery] GetDepartmentTreeQuery query,
        [FromServices] IQueryHandler<GetDepartmentTreeQuery, List<DepartmentTreeNodeDto>> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(query, cancellationToken);

    [HttpGet("tree/search")]
    public async Task<EndpointResult<List<DepartmentSearchResultDto>>> SearchTree(
        [FromQuery] string q,
        [FromServices] IQueryHandler<SearchDepartmentTreeQuery, List<DepartmentSearchResultDto>> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(new SearchDepartmentTreeQuery(q), cancellationToken);

    [HttpGet("{departmentId:guid}/children")]
    public async Task<EndpointResult<List<DepartmentTreeNodeDto>>> GetChildren(
        [FromRoute] Guid departmentId,
        [FromServices] IQueryHandler<GetDepartmentChildrenQuery, List<DepartmentTreeNodeDto>> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(new GetDepartmentChildrenQuery(departmentId), cancellationToken);

    [HttpGet("{departmentId:guid}/ancestors")]
    public async Task<EndpointResult<List<DepartmentTreeNodeDto>>> GetAncestors(
        [FromRoute] Guid departmentId,
        [FromServices] IQueryHandler<GetDepartmentAncestorsQuery, List<DepartmentTreeNodeDto>> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(new GetDepartmentAncestorsQuery(departmentId), cancellationToken);

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

    [HttpDelete]
    [Route("/api/department/{departmentId}")]
    public async Task<EndpointResult<Guid>> SoftDelete(
        [FromRoute] Guid departmentId,
        [FromServices] ICommandHandler<SoftDeleteDepartmentCommand, Guid> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(new SoftDeleteDepartmentCommand(new DepartmentId(departmentId)), cancellationToken);

    [HttpPatch]
    [Route("/api/departments/{departmentId}/move")]
    public async Task<EndpointResult<Guid>> Move(
        [FromRoute] Guid departmentId,
        [FromBody] MoveDepartmentDto moveDepartmentDto,
        [FromServices] ICommandHandler<MoveDepartmentCommand, Guid> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(new MoveDepartmentCommand(departmentId, moveDepartmentDto.NewParentId), cancellationToken);

    [HttpPut]
    [Route("/api/departments/{departmentId}/parent")]
    public async Task<EndpointResult<DepartmentParentDto>> UpdateDepartmentParent(
        [FromRoute] Guid departmentId,
        [FromBody] UpdateDepartmentParentParentDto newParentId,
        [FromServices] ICommandHandler<UpdateDepartmentParentCommand, DepartmentParentDto> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(
            new UpdateDepartmentParentCommand(departmentId,newParentId),
            cancellationToken);


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