using Microsoft.AspNetCore.Mvc;

namespace FastinventorySale.Src.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            service = "fastinventorySale",
            status = "ok",
            timestampUtc = DateTime.UtcNow
        });
    }
}
