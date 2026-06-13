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

    public async Task<MonthlySalesDashboardDto> GetMonthlySalesAsync(string companyCen)
    {
        var now = DateTimeOffset.UtcNow;
        var startCurrent = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero);
        var endCurrent = startCurrent.AddMonths(1).AddTicks(-1);

        var startPrev = startCurrent.AddMonths(-1);
        var endPrev = startCurrent.AddTicks(-1);

        var currentSales = await _saleRepo.GetByMonthlyRangeAsync(companyCen, startCurrent, endCurrent);
        var prevSales = await _saleRepo.GetByMonthlyRangeAsync(companyCen, startPrev, endPrev);

        return new MonthlySalesDashboardDto
        {
            CurrentMonth = MapToSummary(currentSales),
            PreviousMonth = MapToSummary(prevSales)
        };
    }

    private static MonthlySummaryDto MapToSummary(IEnumerable<fastinventorySale.Src.Domain.Entities.Sale> sales)
    {
        var total = (double)sales.Sum(s => s.Total);
        var count = sales.Count();
        return new MonthlySummaryDto { TotalSales = total, TicketsCount = count, AverageTicket = count > 0 ? total / count : 0 };
    }

    public async Task<IEnumerable<TopProductDashboardContractResponse>> GetTopProductsAsync(string companyCen, int topN)
    {
        return new List<TopProductDashboardContractResponse>();
    }

    public async Task<KdsStatusDashboardDto> GetKdsStatusAsync(string companyCen)
    {
        var tickets = await _ticketRepo.GetActiveByCompanyCenAsync(companyCen);
        var items = tickets.SelectMany(t => t.Items).ToList();
        return new KdsStatusDashboardDto
        {
            PendingCount = items.Count(i => i.KdsStatus == "CREATED"),
            PreparingCount = items.Count(i => i.KdsStatus == "PREPARING"),
            ReadyCount = items.Count(i => i.KdsStatus == "READY")
        };
    }
}