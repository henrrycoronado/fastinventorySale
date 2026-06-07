using fastinventorySale.Src.Application.DTOs.Common;
using fastinventorySale.Src.Application.DTOs.Sales;

namespace fastinventorySale.Src.Application.Interfaces;

public interface ICatalogService
{
    Task<IEnumerable<SellableProductContractDto>> GetProductsAsync(string companyCen, SellableProductQueryFilters filters);
}

public interface IDashboardService
{
    Task<DailySalesDashboardDto> GetDailySalesAsync(string companyCen);
    Task<IEnumerable<TopProductDashboardContractResponse>> GetTopProductsAsync(string companyCen, int topN);
    Task<KdsStatusDashboardDto> GetKdsStatusAsync(string companyCen);
}

public interface IKdsService
{
    Task<IEnumerable<KdsTeamContractResponse>> GetTeamsAsync(string companyCen);
    Task<KdsTeamContractResponse> CreateTeamAsync(CreateKdsTeamDto dto);
    Task<IEnumerable<KdsItemContractResponse>> GetItemsByTeamAsync(string companyCen, string teamCen);
    Task UpdateItemStatusAsync(string companyCen, string ticketItemCen, string status);
}

public interface ITicketService
{
    Task<TicketContractResponse> CreateAsync(string companyCen, CreateTicketContractRequest request);
    Task<IEnumerable<TicketContractResponse>> GetActiveByCompanyAsync(string companyCen);
    Task<IEnumerable<TicketItemContractResponse>> GetItemsAsync(string companyCen, string ticketCen);
    Task<TicketItemContractResponse> AddItemAsync(string companyCen, string ticketCen, CreateTicketItemContractRequest request);
    Task<TicketItemContractResponse> UpdateItemAsync(string companyCen, string ticketCen, string ticketItemCen, UpdateTicketItemContractRequest request);
    Task MarkAsSentToKdsAsync(string companyCen, string ticketCen);
    Task ResendToKdsAsync(string companyCen, string ticketCen, string ticketItemCen);
    Task<AssignTicketWaiterContractResponse> AssignWaiterAsync(string companyCen, string ticketCen, AssignTicketWaiterContractRequest request);
    Task<CancelTicketContractResponse> CancelAsync(string companyCen, string ticketCen, CancelTicketContractRequest request);
    Task<TicketTotalsContractResponse> GetTotalsAsync(string companyCen, string ticketCen);
}

public interface ISaleService
{
    Task<PayTicketContractResponse> ProcessPaymentAsync(string companyCen, string ticketCen, PayTicketContractRequest request);
}

public interface IConfigService
{
    Task<TaxConfigurationContractResponse> GetTaxConfigurationAsync(string companyCen);
    Task<TaxConfigurationContractResponse> UpdateTaxConfigurationAsync(string companyCen, UpdateTaxConfigurationContractRequest request);
    Task<IEnumerable<PaymentMethodContractResponse>> GetPaymentMethodsAsync();
    Task<IEnumerable<WaiterContractResponse>> GetWaitersAsync(string companyCen);
    Task<WaiterContractResponse> CreateWaiterAsync(CreateWaiterDto dto);
}
