using prismodSale.Src.Application.DTOs.Common;
using prismodSale.Src.Application.DTOs.Sales;
using prismodSale.Src.Application.Interfaces;
using prismodSale.Src.Infraestructure.ExternalServices;

namespace prismodSale.Src.Application.Services;

public class CatalogService : ICatalogService
{
    private readonly IInventoryClient _inventoryClient;
    public CatalogService(IInventoryClient inventoryClient) => _inventoryClient = inventoryClient;

    public async Task<IEnumerable<SellableProductContractDto>> GetProductsAsync(string companyCen, SellableProductQueryFilters filters)
    {
        return await _inventoryClient.GetSellableProductsAsync(companyCen, filters);
    }
}
