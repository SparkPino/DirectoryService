using DirectoryService.Application.Abstraction;
using DirectoryService.Application.Departments;
using DirectoryService.Application.Departments.RemoveDepartment;
using DirectoryService.Contracts.Department;
using DirectoryService.Presenters.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.EndpointResult;

namespace DirectoryService.Presenters;

[ApiController]
[Route("/api/departments/[action]")]
[Produces("application/json")]
public class DepartmentController : BaseApiController
{
    [ProducesResponseType(typeof(Envelope<Guid>), StatusCodes.Status200OK)]
    [HttpPost]
    //[Route("/api/departments/[action]")]
    public async Task<EndpointResult<Guid>> Create(
        [FromBody] DepartmentDto departmentDto,
        [FromServices] ICommandHandler<AddDepartmentCommand, Guid> handler,
        CancellationToken cancellationToken) =>
        await handler.Handle(new AddDepartmentCommand(departmentDto), cancellationToken);

    [HttpDelete]
    public async Task<EndpointResult<Unit>> Delete(
        [FromBody] Guid departmentId,
        [FromServices] ICommandHandler<RemoveDepartmentCommand, Unit> handler,
        CancellationToken cancellationToken)
    {
       return await handler.Handle(new RemoveDepartmentCommand(departmentId), cancellationToken);
    }
}