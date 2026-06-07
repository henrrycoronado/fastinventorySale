namespace fastinventorySale.Src.Infraestructure.Persistence.Models;

public class TaxConfigurationModel
{
    public long Id { get; set; }
    public string CompanyCen { get; set; } = string.Empty;
    public decimal GlobalTaxPercentage { get; set; }
}

public class PaymentMethodModel
{
    public string PaymentMethodCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class WaiterModel
{
    public long Id { get; set; }
    public string WaiterCen { get; set; } = string.Empty;
    public string CompanyCen { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class KdsTeamModel
{
    public long Id { get; set; }
    public string TeamCen { get; set; } = string.Empty;
    public string CompanyCen { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public ICollection<KdsTeamCategoryModel> Categories { get; set; } = new List<KdsTeamCategoryModel>();
}

public class KdsTeamCategoryModel
{
    public string TeamCen { get; set; } = string.Empty;
    public string CategoryCen { get; set; } = string.Empty;

    public KdsTeamModel Team { get; set; } = null!;
}

public class TicketModel
{
    public long Id { get; set; }
    public string TicketCen { get; set; } = string.Empty;
    public string CompanyCen { get; set; } = string.Empty;
    public string? WaiterCen { get; set; }
    public int DailyNumber { get; set; }
    public string Status { get; set; } = "OPEN";
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public ICollection<TicketItemModel> Items { get; set; } = new List<TicketItemModel>();
}

public class TicketItemModel
{
    public long Id { get; set; }
    public string TicketItemCen { get; set; } = string.Empty;
    public string TicketCen { get; set; } = string.Empty;
    public string ProductCen { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Note { get; set; }
    public string KdsStatus { get; set; } = "CREATED";
    public DateTimeOffset? SentAt { get; set; }
    public int ResendCount { get; set; }

    public TicketModel Ticket { get; set; } = null!;
}

public class SaleModel
{
    public long Id { get; set; }
    public string SaleCen { get; set; } = string.Empty;
    public string TicketCen { get; set; } = string.Empty;
    public string? PaymentMethodCode { get; set; }
    public string? InventoryDocumentCen { get; set; }
    public decimal Total { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
