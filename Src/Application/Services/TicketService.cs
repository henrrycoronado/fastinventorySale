using fastinventorySale.Src.Application.DTOs.Sales;
using fastinventorySale.Src.Application.Interfaces;
using fastinventorySale.Src.Domain.Entities;
using fastinventorySale.Src.Infraestructure.Persistence.Interfaces;

namespace fastinventorySale.Src.Application.Services;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _ticketRepo;
    private readonly ITaxConfigurationRepository _taxRepo;
    private readonly IWaiterRepository _waiterRepo;
    private readonly IUnitOfWork _uow;

    public TicketService(ITicketRepository ticketRepo, ITaxConfigurationRepository taxRepo, IWaiterRepository waiterRepo, IUnitOfWork uow)
    {
        _ticketRepo = ticketRepo;
        _taxRepo = taxRepo;
        _waiterRepo = waiterRepo;
        _uow = uow;
    }

    public async Task<TicketContractResponse> CreateAsync(string companyCen, CreateTicketContractRequest request)
    {
        var dailyNumber = await _ticketRepo.GetNextDailyNumberAsync(companyCen);
        var ticket = new Ticket(companyCen, dailyNumber, request.WaiterCen);
        await _ticketRepo.AddAsync(ticket);
        await _uow.SaveChangesAsync();
        return MapToDto(ticket);
    }

    public async Task<IEnumerable<TicketContractResponse>> GetActiveByCompanyAsync(string companyCen)
    {
        var tickets = await _ticketRepo.GetActiveByCompanyCenAsync(companyCen);
        return tickets.Select(MapToDto);
    }

    public async Task<IEnumerable<TicketItemContractResponse>> GetItemsAsync(string companyCen, string ticketCen)
    {
        var ticket = await _ticketRepo.GetByCenAsync(ticketCen);
        if (ticket == null) throw new KeyNotFoundException("Ticket not found");
        return ticket.Items.Select(i => MapToItemDto(i));
    }

    public async Task<TicketItemContractResponse> AddItemAsync(string companyCen, string ticketCen, CreateTicketItemContractRequest request)
    {
        var ticket = await _ticketRepo.GetByCenAsync(ticketCen);
        if (ticket == null) throw new KeyNotFoundException("Ticket not found");
        if (ticket.Status != "OPEN") throw new InvalidOperationException("Ticket is not open");

        var item = new TicketItem(request.ProductCen, request.Quantity, 0, request.Note); // Price should come from Catalog/Inventory
                                                                                          // For now, setting price to 0, needs integration lookup

        await _ticketRepo.AddItemAsync(ticketCen, item);
        await UpdateTotals(ticket, companyCen);
        return MapToItemDto(item);
    }

    public async Task<TicketItemContractResponse> UpdateItemAsync(string companyCen, string ticketCen, string ticketItemCen, UpdateTicketItemContractRequest request)
    {
        var item = await _ticketRepo.GetItemByCenAsync(ticketItemCen);
        if (item == null) throw new KeyNotFoundException("Item not found");

        item.UpdateQuantity(request.Quantity);
        item.UpdateNote(request.Note);

        await _ticketRepo.UpdateItemAsync(item);

        var ticket = await _ticketRepo.GetByCenAsync(ticketCen);
        if (ticket != null) await UpdateTotals(ticket, companyCen);

        return MapToItemDto(item);
    }

    public async Task<IEnumerable<TicketItemContractResponse>> MarkAsSentToKdsAsync(string companyCen, string ticketCen)
    {
        var ticket = await _ticketRepo.GetByCenAsync(ticketCen);
        if (ticket == null) throw new KeyNotFoundException("Ticket not found");
        
        var sentItems = new List<TicketItemContractResponse>();
        foreach (var item in ticket.Items.Where(i => i.KdsStatus == "CREATED"))
        {
            item.MarkAsSent();
            await _ticketRepo.UpdateItemAsync(item);
            sentItems.Add(MapToItemDto(item));
        }
        await _uow.SaveChangesAsync();
        return sentItems;
    }

    public async Task<TicketItemContractResponse> ResendToKdsAsync(string companyCen, string ticketCen, string ticketItemCen)
    {
        var item = await _ticketRepo.GetItemByCenAsync(ticketItemCen);
        if (item == null) throw new KeyNotFoundException("Item not found");

        item.IncrementResend();
        await _ticketRepo.UpdateItemAsync(item);
        await _uow.SaveChangesAsync();
        
        return MapToItemDto(item);
    }

    public async Task<byte[]> PrintTicketAsync(string companyCen, string ticketCen)
    {
        var ticket = await _ticketRepo.GetByCenAsync(ticketCen);
        if (ticket == null) throw new KeyNotFoundException("Ticket not found");

        // Placeholder for PDF generation
        return System.Text.Encoding.UTF8.GetBytes($"Ticket: {ticket.TicketCen}\nDaily Number: {ticket.DailyNumber}\nTotal: {ticket.Total}");
    }

    public async Task<AssignTicketWaiterContractResponse> AssignWaiterAsync(string companyCen, string ticketCen, AssignTicketWaiterContractRequest request)
    {
        var ticket = await _ticketRepo.GetByCenAsync(ticketCen);
        if (ticket == null) throw new KeyNotFoundException("Ticket not found");

        var waiter = await _waiterRepo.GetByCenAsync(request.WaiterCen);
        if (waiter == null) throw new KeyNotFoundException("Waiter not found");

        ticket.AssignWaiter(request.WaiterCen);
        await _ticketRepo.UpdateAsync(ticket);
        await _uow.SaveChangesAsync();

        return new AssignTicketWaiterContractResponse { TicketCen = ticketCen, WaiterCen = waiter.WaiterCen, WaiterName = waiter.Name };
    }

    public async Task<CancelTicketContractResponse> CancelAsync(string companyCen, string ticketCen, CancelTicketContractRequest request)
    {
        var ticket = await _ticketRepo.GetByCenAsync(ticketCen);
        if (ticket == null) throw new KeyNotFoundException("Ticket not found");

        ticket.Cancel();
        await _ticketRepo.UpdateAsync(ticket);
        await _uow.SaveChangesAsync();

        return new CancelTicketContractResponse { TicketCen = ticketCen, Status = ticket.Status };
    }

    public async Task<TicketTotalsContractResponse> GetTotalsAsync(string companyCen, string ticketCen)
    {
        var ticket = await _ticketRepo.GetByCenAsync(ticketCen);
        if (ticket == null) throw new KeyNotFoundException("Ticket not found");
        return new TicketTotalsContractResponse { Subtotal = ticket.Subtotal, TaxAmount = ticket.TaxAmount, Total = ticket.Total };
    }

    private async Task UpdateTotals(Ticket ticket, string companyCen)
    {
        var config = await _taxRepo.GetByCompanyCenAsync(companyCen);
        ticket.RecalculateTotals(config?.GlobalTaxPercentage ?? 0);
        await _ticketRepo.UpdateAsync(ticket);
        await _uow.SaveChangesAsync();
    }

    private static TicketContractResponse MapToDto(Ticket t) => new() { TicketCen = t.TicketCen, DailyNumber = t.DailyNumber, Status = t.Status, WaiterCen = t.WaiterCen, CreatedAt = t.CreatedAt, ItemCount = t.Items.Count, Total = t.Total };
    private static TicketItemContractResponse MapToItemDto(TicketItem i) => new() { TicketItemCen = i.TicketItemCen, ProductCen = i.ProductCen, Quantity = i.Quantity, UnitPrice = i.UnitPrice, Subtotal = i.Quantity * i.UnitPrice, Note = i.Note, KdsStatus = i.KdsStatus };
}
