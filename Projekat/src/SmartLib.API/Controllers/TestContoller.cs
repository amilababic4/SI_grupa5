using Microsoft.AspNetCore.Mvc;

namespace SmartLib.API.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("API radi!");
    }
}