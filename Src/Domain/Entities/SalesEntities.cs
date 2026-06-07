namespace prismodSale.Src.Domain.Entities;

public class TaxConfiguration
{
    public string CompanyCen { get; private set; }
    public decimal GlobalTaxPercentage { get; private set; }

    public TaxConfiguration(string companyCen, decimal globalTaxPercentage)
    {
        CompanyCen = companyCen;
        GlobalTaxPercentage = globalTaxPercentage;
    }

    public void UpdatePercentage(decimal newPercentage) => GlobalTaxPercentage = newPercentage;
}

public class PaymentMethod
{
    public string PaymentMethodCode { get; private set; }
    public string Name { get; private set; }
    public bool IsActive { get; private set; }

    public PaymentMethod(string code, string name)
    {
        PaymentMethodCode = code;
        Name = name;
        IsActive = true;
    }
}

public class Waiter
{
    public string WaiterCen { get; private set; }
    public string CompanyCen { get; private set; }
    public string Name { get; private set; }

    public Waiter(string companyCen, string name)
    {
        WaiterCen = Guid.NewGuid().ToString("N");
        CompanyCen = companyCen;
        Name = name;
    }
}

public class KdsTeam
{
    public string TeamCen { get; private set; }
    public string CompanyCen { get; private set; }
    public string Name { get; private set; }

    private readonly List<string> _categoryCens = new();
    public IReadOnlyCollection<string> CategoryCens => _categoryCens.AsReadOnly();

    public KdsTeam(string companyCen, string name)
    {
        TeamCen = Guid.NewGuid().ToString("N");
        CompanyCen = companyCen;
        Name = name;
    }

    public void AddCategory(string categoryCen) => _categoryCens.Add(categoryCen);
}

public class Ticket
{
    public string TicketCen { get; private set; }
    public string CompanyCen { get; private set; }
    public string? WaiterCen { get; private set; }
    public int DailyNumber { get; private set; }
    public string Status { get; private set; } // OPEN, CLOSED, CANCELED
    public decimal Subtotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal Total { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private readonly List<TicketItem> _items = new();
    public IReadOnlyCollection<TicketItem> Items => _items.AsReadOnly();

    public Ticket(string companyCen, int dailyNumber, string? waiterCen = null)
    {
        TicketCen = Guid.NewGuid().ToString("N");
        CompanyCen = companyCen;
        DailyNumber = dailyNumber;
        WaiterCen = waiterCen;
        Status = "OPEN";
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void AddItem(string productCen, int quantity, decimal unitPrice, string? note = null)
    {
        _items.Add(new TicketItem(productCen, quantity, unitPrice, note));
        RecalculateTotals(0); // Needs tax percentage from service/app layer
    }

    public void RecalculateTotals(decimal taxPercentage)
    {
        Subtotal = _items.Sum(i => i.Quantity * i.UnitPrice);
        TaxAmount = Subtotal * (taxPercentage / 100);
        Total = Subtotal + TaxAmount;
    }

    public void AssignWaiter(string waiterCen) => WaiterCen = waiterCen;
    public void Cancel() => Status = "CANCELED";
    public void Close() => Status = "CLOSED";
}

public class TicketItem
{
    public string TicketItemCen { get; private set; }
    public string ProductCen { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public string? Note { get; private set; }
    public string KdsStatus { get; private set; } // CREATED, PREPARING, DELIVERED, CANCELED
    public DateTimeOffset? SentAt { get; private set; }
    public int ResendCount { get; private set; }

    public TicketItem(string productCen, int quantity, decimal unitPrice, string? note = null)
    {
        TicketItemCen = Guid.NewGuid().ToString("N");
        ProductCen = productCen;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Note = note;
        KdsStatus = "CREATED";
    }

    public void UpdateQuantity(int newQuantity) => Quantity = newQuantity;
    public void UpdateNote(string? newNote) => Note = newNote;
    public void SetKdsStatus(string status) => KdsStatus = status;
    public void MarkAsSent() { SentAt = DateTimeOffset.UtcNow; KdsStatus = "PREPARING"; }
    public void IncrementResend() => ResendCount++;
}

public class Sale
{
    public string SaleCen { get; private set; }
    public string TicketCen { get; private set; }
    public string? PaymentMethodCode { get; private set; }
    public string? InventoryDocumentCen { get; private set; }
    public decimal Total { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public Sale(string ticketCen, decimal total, string? paymentMethodCode = null, string? inventoryDocumentCen = null)
    {
        SaleCen = Guid.NewGuid().ToString("N");
        TicketCen = ticketCen;
        Total = total;
        PaymentMethodCode = paymentMethodCode;
        InventoryDocumentCen = inventoryDocumentCen;
        CreatedAt = DateTimeOffset.UtcNow;
    }
}
