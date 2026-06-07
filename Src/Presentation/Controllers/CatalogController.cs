using Microsoft.AspNetCore.Mvc;
using prismodSale.Src.Application.DTOs.Common;
using prismodSale.Src.Application.DTOs.Sales;
using prismodSale.Src.Application.Interfaces;

namespace prismodSale.Src.Presentation.Controllers;

[ApiController]
[Route("api/sales/companies/{companyCen}/catalog")]
public class CatalogController : ControllerBase
{
    private readonly ICatalogService _catalogService;
    public CatalogController(ICatalogService catalogService) => _catalogService = catalogService;

    [HttpGet("products")]
    public async Task<ActionResult<IEnumerable<SellableProductContractDto>>> GetProducts(
        string companyCen,
        [FromQuery] string? search,
        [FromQuery] string? categoryCen,
        [FromQuery] string? warehouseCen,
        [FromQuery] bool onlyAvailable = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var filters = new SellableProductQueryFilters { Search = search, CategoryCen = categoryCen, WarehouseCen = warehouseCen, OnlyAvailable = onlyAvailable, Page = page, PageSize = pageSize };
        return Ok(await _catalogService.GetProductsAsync(companyCen, filters));
    }
}
