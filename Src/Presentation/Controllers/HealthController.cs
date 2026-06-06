using Microsoft.AspNetCore.Mvc;

namespace PrismodSale.Src.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            service = "prismodSale",
            status = "ok",
            timestampUtc = DateTime.UtcNow
        });
    }
}
