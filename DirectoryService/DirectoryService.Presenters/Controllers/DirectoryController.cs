namespace DirectoryService.Presentation.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class DirectoryController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok("blabla");
    }
}