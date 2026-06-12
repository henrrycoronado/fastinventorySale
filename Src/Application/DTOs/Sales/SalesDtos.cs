namespace fastinventorySale.Src.Application.DTOs.Sales;

public class SellableProductContractDto
{
    public string ProductCen { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? CategoryCen { get; set; }
    public string? CategoryName { get; set; }
    public decimal SalePrice { get; set; }
    public decimal AvailableQuantity { get; set; }
    public bool IsAvailable { get; set; }
    public string? StationCode { get; set; }
}

public class DailySalesDashboardDto
{
    public double TotalSales { get; set; }
    public int TicketsCount { get; set; }
    public double AverageTicket { get; set; }
}

public class TopProductDashboardContractResponse
{
    public string ProductCen { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public double TotalAmount { get; set; }
}

public class KdsStatusDashboardDto
{
    public int PendingItems { get; set; }
    public int PreparingItems { get; set; }
    public double AverageWaitTimeMinutes { get; set; }
}

public class KdsTeamContractResponse
{
    public string TeamCen { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<string> CategoryCens { get; set; } = new();
}

public class KdsItemContractResponse
{
    public string TicketItemCen { get; set; } = string.Empty;
    public string TicketCen { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset OrderedAt { get; set; }
    public string? Note { get; set; }
}

public class UpdateKdsItemStatusContractRequest
{
    public string Status { get; set; } = string.Empty;
}

public class PaymentMethodContractResponse
{
    public string PaymentMethodCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class TaxConfigurationContractResponse
{
    public string CompanyCen { get; set; } = string.Empty;
    public decimal GlobalTaxPercentage { get; set; }
}

public class UpdateTaxConfigurationContractRequest
{
    public decimal GlobalTaxPercentage { get; set; }
}

public class CreateTicketContractRequest
{
    public string? WaiterCen { get; set; }
}

public class TicketContractResponse
{
    public string TicketCen { get; set; } = string.Empty;
    public int DailyNumber { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? WaiterCen { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public int ItemCount { get; set; }
    public decimal Total { get; set; }
}

public class TicketItemContractResponse
{
    public string TicketItemCen { get; set; } = string.Empty;
    public string ProductCen { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
    public string? Note { get; set; }
    public string KdsStatus { get; set; } = string.Empty;
}

public class CreateTicketItemContractRequest
{
    public string ProductCen { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string? Note { get; set; }
}

public class UpdateTicketItemContractRequest
{
    public int Quantity { get; set; }
    public string? Note { get; set; }
}

public class AssignTicketWaiterContractRequest
{
    public string WaiterCen { get; set; } = string.Empty;
}

public class AssignTicketWaiterContractResponse
{
    public string TicketCen { get; set; } = string.Empty;
    public string WaiterCen { get; set; } = string.Empty;
    public string WaiterName { get; set; } = string.Empty;
}

public class CancelTicketContractRequest
{
    public string? Reason { get; set; }
}

public class CancelTicketContractResponse
{
    public string TicketCen { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class TicketTotalsContractResponse
{
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
}

public class PayTicketContractRequest
{
    public string PaymentMethodCode { get; set; } = string.Empty;
}

public class PayTicketContractResponse
{
    public string SaleCen { get; set; } = string.Empty;
    public string TicketCen { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? InventoryDocumentCen { get; set; }
}

public class WaiterContractResponse
{
    public string WaiterCen { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class CreateWaiterDto
{
    public string CompanyCen { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class CreateKdsTeamDto
{
    public string CompanyCen { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<string> CategoryCens { get; set; } = new();
}