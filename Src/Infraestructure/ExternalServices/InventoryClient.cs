using System.Text.Json;

using fastinventorySale.Src.Application.DTOs.Common;
using fastinventorySale.Src.Application.DTOs.Sales;

namespace fastinventorySale.Src.Infraestructure.ExternalServices;

public class StockValidationItemDto
{
    public string ProductCen { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
}

public class StockValidationRequestDto
{
    public string WarehouseCen { get; set; } = string.Empty;
    public string? Source { get; set; }
    public string? ReferenceCen { get; set; }
    public string? Reason { get; set; }
    public List<StockValidationItemDto> Items { get; set; } = new();
}

public interface IInventoryClient
{
    Task<IEnumerable<SellableProductContractDto>> GetSellableProductsAsync(string companyCen, SellableProductQueryFilters filters);
    Task<string?> ConsumeStockAsync(string companyCen, StockValidationRequestDto request);
}

public class InventoryClient : IInventoryClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<InventoryClient> _logger;

    public InventoryClient(HttpClient httpClient, ILogger<InventoryClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IEnumerable<SellableProductContractDto>> GetSellableProductsAsync(string companyCen, SellableProductQueryFilters filters)
    {
        var query = $"?search={filters.Search}&categoryCen={filters.CategoryCen}&warehouseCen={filters.WarehouseCen}&onlyAvailable={filters.OnlyAvailable}&page={filters.Page}&pageSize={filters.PageSize}";
        var response = await _httpClient.GetAsync($"/api/inventory/companies/{companyCen}/sellable-products{query}");

        if (!response.IsSuccessStatusCode) return Enumerable.Empty<SellableProductContractDto>();

        return await response.Content.ReadFromJsonAsync<IEnumerable<SellableProductContractDto>>() ?? Enumerable.Empty<SellableProductContractDto>();
    }

    public async Task<string?> ConsumeStockAsync(string companyCen, StockValidationRequestDto request)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/inventory/companies/{companyCen}/stock/consume", request);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("Failed to consume stock in Inventory API: {Error}", error);
            return null;
        }

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        return result.GetProperty("documentCen").GetString();
    }
}