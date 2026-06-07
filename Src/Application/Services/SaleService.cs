using prismodSale.Src.Application.DTOs.Sales;
using prismodSale.Src.Application.Interfaces;
using prismodSale.Src.Domain.Entities;
using prismodSale.Src.Infraestructure.Persistence.Interfaces;
using prismodSale.Src.Infraestructure.ExternalServices;

namespace prismodSale.Src.Application.Services;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepo;
    private readonly ITicketRepository _ticketRepo;
    private readonly IInventoryClient _inventoryClient;
    private readonly IUnitOfWork _uow;

    public SaleService(ISaleRepository saleRepo, ITicketRepository ticketRepo, IInventoryClient inventoryClient, IUnitOfWork uow)
    {
        _saleRepo = saleRepo;
        _ticketRepo = ticketRepo;
        _inventoryClient = inventoryClient;
        _uow = uow;
    }

    public async Task<PayTicketContractResponse> ProcessPaymentAsync(string companyCen, string ticketCen, PayTicketContractRequest request)
    {
        var ticket = await _ticketRepo.GetByCenAsync(ticketCen);
        if (ticket == null) throw new KeyNotFoundException("Ticket not found");
        if (ticket.Status != "OPEN") throw new InvalidOperationException("Ticket is already closed or canceled");

        // 1. Consume stock in Inventory
        var stockRequest = new StockValidationRequestDto
        {
            WarehouseCen = "CEN-WH-001", // Should be configured per company/station
            Source = "SALE",
            ReferenceCen = ticket.TicketCen,
            Items = ticket.Items.Select(i => new StockValidationItemDto { ProductCen = i.ProductCen, Quantity = i.Quantity }).ToList()
        };

        var inventoryDocCen = await _inventoryClient.ConsumeStockAsync(companyCen, stockRequest);
        
        // 2. Create Sale record
        var sale = new Sale(ticketCen, ticket.Total, request.PaymentMethodCode, inventoryDocCen);
        await _saleRepo.AddAsync(sale);

        // 3. Close ticket
        ticket.Close();
        await _ticketRepo.UpdateAsync(ticket);
        await _uow.SaveChangesAsync();

        return new PayTicketContractResponse
        {
            SaleCen = sale.SaleCen,
            TicketCen = ticket.TicketCen,
            Status = ticket.Status,
            InventoryDocumentCen = inventoryDocCen
        };
    }
}
