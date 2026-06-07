using fastinventorySale.Src.Application.DTOs.Sales;
using fastinventorySale.Src.Application.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace fastinventorySale.Src.Presentation.Controllers;

[ApiController]
[Route("api/sales/companies/{companyCen}/waiters")]
public class WaitersController : ControllerBase
{
    private readonly IConfigService _configService;
    public WaitersController(IConfigService configService) => _configService = configService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WaiterContractResponse>>> GetAll(string companyCen) => Ok(await _configService.GetWaitersAsync(companyCen));

    [HttpPost]
    public async Task<ActionResult<WaiterContractResponse>> Create(string companyCen, [FromBody] CreateWaiterDto dto)
    {
        dto.CompanyCen = companyCen;
        var waiter = await _configService.CreateWaiterAsync(dto);
        return CreatedAtAction(nameof(GetAll), new { companyCen }, waiter);
    }
}
