using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace DirectoryService.Presenters.Controllers;

[ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(Envelope), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(Envelope), StatusCodes.Status500InternalServerError)]
public class BaseApiController : ControllerBase
{
}