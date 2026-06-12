namespace fastinventorySale.Src.Application.DTOs.Sales;

public class MonthlySalesDashboardDto
{
    public MonthlySummaryDto CurrentMonth { get; set; } = new();
    public MonthlySummaryDto PreviousMonth { get; set; } = new();
}

public class MonthlySummaryDto
{
    public double TotalSales { get; set; }
    public int TicketsCount { get; set; }
    public double AverageTicket { get; set; }
}
