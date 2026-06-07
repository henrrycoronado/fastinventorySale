using fastinventorySale.Src.Application.DTOs.Sales;
using fastinventorySale.Src.Application.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace fastinventorySale.Src.Presentation.Controllers;

[ApiController]
[Route("api/sales/companies/{companyCen}/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    public DashboardController(IDashboardService dashboardService) => _dashboardService = dashboardService;

    [HttpGet("daily-sales")]
    public async Task<ActionResult<DailySalesDashboardDto>> GetDailySales(string companyCen) => Ok(await _dashboardService.GetDailySalesAsync(companyCen));

    [HttpGet("top-products")]
    public async Task<ActionResult<IEnumerable<TopProductDashboardContractResponse>>> GetTopProducts(string companyCen, [FromQuery] int topN = 10) => Ok(await _dashboardService.GetTopProductsAsync(companyCen, topN));

    [HttpGet("kds-status")]
    public async Task<ActionResult<KdsStatusDashboardDto>> GetKdsStatus(string companyCen) => Ok(await _dashboardService.GetKdsStatusAsync(companyCen));
}
