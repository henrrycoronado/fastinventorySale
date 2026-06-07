using fastinventorySale.Src.Application.DTOs.Sales;
using fastinventorySale.Src.Application.Interfaces;
using fastinventorySale.Src.Infraestructure.Persistence.Interfaces;

namespace fastinventorySale.Src.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly ISaleRepository _saleRepo;
    private readonly ITicketRepository _ticketRepo;

    public DashboardService(ISaleRepository saleRepo, ITicketRepository ticketRepo)
    {
        _saleRepo = saleRepo;
        _ticketRepo = ticketRepo;
    }

    public async Task<DailySalesDashboardDto> GetDailySalesAsync(string companyCen)
    {
        var sales = await _saleRepo.GetDailyByCompanyCenAsync(companyCen, DateTimeOffset.UtcNow);
        var total = (double)sales.Sum(s => s.Total);
        var count = sales.Count();
        return new DailySalesDashboardDto { TotalSales = total, TicketsCount = count, AverageTicket = count > 0 ? total / count : 0 };
    }

    public async Task<IEnumerable<TopProductDashboardContractResponse>> GetTopProductsAsync(string companyCen, int topN)
    {
        // This would normally be a complex SQL query. For now, mock or simple agg.
        return new List<TopProductDashboardContractResponse>();
    }

    public async Task<KdsStatusDashboardDto> GetKdsStatusAsync(string companyCen)
    {
        var tickets = await _ticketRepo.GetActiveByCompanyCenAsync(companyCen);
        var items = tickets.SelectMany(t => t.Items).ToList();
        return new KdsStatusDashboardDto
        {
            PendingItems = items.Count(i => i.KdsStatus == "CREATED"),
            PreparingItems = items.Count(i => i.KdsStatus == "PREPARING"),
            AverageWaitTimeMinutes = 0
        };
    }
}
