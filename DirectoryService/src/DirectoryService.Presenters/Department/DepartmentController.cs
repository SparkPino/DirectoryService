using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Departments;
using DirectoryService.Application.Departments.AddDepartment;
using DirectoryService.Application.Departments.AttachLocationToDepartment;
using DirectoryService.Application.Departments.DetachLocationFromDepartment;
using DirectoryService.Application.Departments.RemoveDepartment;
using DirectoryService.Application.Departments.UpdateDepartment;
using DirectoryService.Contracts.Department;
using DirectoryService.Presenters.Controllers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.EndpointResult;
using Unit = Shared.Unit;

namespace DirectoryService.Presenters.Department;

[ApiController]
[Route("/api/departments/")]
[Produces("application/json")]
public class DepartmentController : BaseApiController
{
    [ProducesResponseType(typeof(Envelope<Guid>), StatusCodes.Status200OK)]
    [HttpPost]
    public async Task<EndpointResult<Guid>> Create(
        [FromBody] DepartmentDto departmentDto,
        [FromServices] ISender sender,
        CancellationToken cancellationToken) =>
        await sender.Send(new AddDepartmentCommand(departmentDto), cancellationToken);

    [HttpDelete]
    public async Task<EndpointResult<Unit>> Delete(
        [FromBody] Guid departmentId,
        [FromServices]  ISender sender,
        CancellationToken cancellationToken) =>
        await sender.Send(new RemoveDepartmentCommand(departmentId), cancellationToken);

    [HttpPatch]
    [Route("/api/departments/{departmentId}")]
    public async Task<EndpointResult<Guid>> Update(
        [FromBody] UpdateDepartmentDto departmentDto,
        [FromRoute] Guid departmentId,
        [FromServices] ISender sender,
        CancellationToken cancellationToken) =>
        await sender.Send(new UpdateDepartmentCommand(departmentDto, departmentId), cancellationToken);

    [HttpPost]
    [Route("/api/departments/{departmentId}/locations/{locationId}")]
    public async Task<EndpointResult<Guid>> AttachLocation(
        [FromRoute] Guid departmentId,
        [FromRoute] Guid locationId,
        [FromServices] ISender sender,
        CancellationToken cancellationToken) =>
        await sender.Send(new AttachLocationToDepartmentCommand(departmentId, locationId), cancellationToken);


    [HttpDelete]
    [Route("/api/departments/{departmentId}/locations/{locationId}")]
    public async Task<EndpointResult<Guid>> DetachLocation(
        [FromRoute] Guid departmentId,
        [FromRoute] Guid locationId,
        [FromServices] ISender sender,
        CancellationToken cancellationToken) =>
        await sender.Send(new DetachLocationFromDepartmentCommand(departmentId, locationId), cancellationToken);
}