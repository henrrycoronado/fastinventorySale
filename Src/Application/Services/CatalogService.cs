using fastinventorySale.Src.Application.DTOs.Common;
using fastinventorySale.Src.Application.DTOs.Sales;
using fastinventorySale.Src.Application.Interfaces;
using fastinventorySale.Src.Infraestructure.ExternalServices;

namespace fastinventorySale.Src.Application.Services;

public class CatalogService : ICatalogService
{
    private readonly IInventoryClient _inventoryClient;
    public CatalogService(IInventoryClient inventoryClient) => _inventoryClient = inventoryClient;

    public async Task<IEnumerable<SellableProductContractDto>> GetProductsAsync(string companyCen, SellableProductQueryFilters filters)
    {
        return await _inventoryClient.GetSellableProductsAsync(companyCen, filters);
    }
}