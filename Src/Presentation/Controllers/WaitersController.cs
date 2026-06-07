using Microsoft.AspNetCore.Mvc;
using prismodSale.Src.Application.DTOs.Sales;
using prismodSale.Src.Application.Interfaces;

namespace prismodSale.Src.Presentation.Controllers;

[ApiController]
[Route("api/sales/companies/{companyCen}/waiters")]
public class WaitersController : ControllerBase
{
    private readonly IConfigService _configService;
    public WaitersController(IConfigService configService) => _configService = configService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WaiterContractResponse>>> GetAll(string companyCen) => Ok(await _configService.GetWaitersAsync(companyCen));
}
