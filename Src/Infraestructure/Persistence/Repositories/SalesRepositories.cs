using fastinventorySale.Src.Domain.Entities;
using fastinventorySale.Src.Infraestructure.Persistence.Interfaces;
using fastinventorySale.Src.Infraestructure.Persistence.Models;

using Microsoft.EntityFrameworkCore;

namespace fastinventorySale.Src.Infraestructure.Persistence.Repositories;

public class TaxConfigurationRepository : ITaxConfigurationRepository
{
    private readonly ApplicationDbContext _dbContext;
    public TaxConfigurationRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public async Task<TaxConfiguration?> GetByCompanyCenAsync(string companyCen)
    {
        var model = await _dbContext.TaxConfigurations.AsNoTracking().FirstOrDefaultAsync(t => t.CompanyCen == companyCen);
        return model == null ? null : new TaxConfiguration(model.CompanyCen, model.GlobalTaxPercentage);
    }
    public async Task AddAsync(TaxConfiguration config) => await _dbContext.TaxConfigurations.AddAsync(new TaxConfigurationModel { CompanyCen = config.CompanyCen, GlobalTaxPercentage = config.GlobalTaxPercentage });
    public async Task UpdateAsync(TaxConfiguration config)
    {
        var model = await _dbContext.TaxConfigurations.FirstOrDefaultAsync(t => t.CompanyCen == config.CompanyCen);
        if (model != null) { model.GlobalTaxPercentage = config.GlobalTaxPercentage; _dbContext.TaxConfigurations.Update(model); }
    }
}

public class PaymentMethodRepository : IPaymentMethodRepository
{
    private readonly ApplicationDbContext _dbContext;
    public PaymentMethodRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public async Task<IEnumerable<PaymentMethod>> GetAllAsync() => await _dbContext.PaymentMethods.AsNoTracking().Select(m => new PaymentMethod(m.PaymentMethodCode, m.Name)).ToListAsync();
    public async Task<PaymentMethod?> GetByCodeAsync(string code)
    {
        var m = await _dbContext.PaymentMethods.AsNoTracking().FirstOrDefaultAsync(x => x.PaymentMethodCode == code);
        return m == null ? null : new PaymentMethod(m.PaymentMethodCode, m.Name);
    }
}

public class WaiterRepository : IWaiterRepository
{
    private readonly ApplicationDbContext _dbContext;
    public WaiterRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public async Task<IEnumerable<Waiter>> GetByCompanyCenAsync(string companyCen) => (await _dbContext.Waiters.AsNoTracking().Where(w => w.CompanyCen == companyCen).ToListAsync()).Select(m => MapToDomain(m));
    public async Task<Waiter?> GetByCenAsync(string waiterCen)
    {
        var m = await _dbContext.Waiters.AsNoTracking().FirstOrDefaultAsync(x => x.WaiterCen == waiterCen);
        return m == null ? null : MapToDomain(m);
    }
    public async Task AddAsync(Waiter waiter) => await _dbContext.Waiters.AddAsync(new WaiterModel { WaiterCen = waiter.WaiterCen, CompanyCen = waiter.CompanyCen, Name = waiter.Name });
    private static Waiter MapToDomain(WaiterModel m)
    {
        var w = new Waiter(m.CompanyCen, m.Name);
        typeof(Waiter).GetProperty(nameof(Waiter.WaiterCen))?.SetValue(w, m.WaiterCen);
        return w;
    }
}

public class KdsTeamRepository : IKdsTeamRepository
{
    private readonly ApplicationDbContext _dbContext;
    public KdsTeamRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public async Task<IEnumerable<KdsTeam>> GetByCompanyCenAsync(string companyCen)
    {
        var models = await _dbContext.KdsTeams.Include(t => t.Categories).AsNoTracking().Where(t => t.CompanyCen == companyCen).ToListAsync();
        return models.Select(m => MapToDomain(m));
    }
    public async Task<KdsTeam?> GetByCenAsync(string teamCen)
    {
        var m = await _dbContext.KdsTeams.Include(t => t.Categories).AsNoTracking().FirstOrDefaultAsync(x => x.TeamCen == teamCen);
        return m == null ? null : MapToDomain(m);
    }
    public async Task AddAsync(KdsTeam team)
    {
        var m = new KdsTeamModel { TeamCen = team.TeamCen, CompanyCen = team.CompanyCen, Name = team.Name };
        foreach (var cat in team.CategoryCens) m.Categories.Add(new KdsTeamCategoryModel { TeamCen = team.TeamCen, CategoryCen = cat });
        await _dbContext.KdsTeams.AddAsync(m);
    }
    private static KdsTeam MapToDomain(KdsTeamModel m)
    {
        var t = new KdsTeam(m.CompanyCen, m.Name);
        typeof(KdsTeam).GetProperty(nameof(KdsTeam.TeamCen))?.SetValue(t, m.TeamCen);
        foreach (var cat in m.Categories) t.AddCategory(cat.CategoryCen);
        return t;
    }
}

public class TicketRepository : ITicketRepository
{
    private readonly ApplicationDbContext _dbContext;
    public TicketRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public async Task<Ticket?> GetByCenAsync(string ticketCen)
    {
        var m = await _dbContext.Tickets.Include(t => t.Items).AsNoTracking().FirstOrDefaultAsync(x => x.TicketCen == ticketCen);
        return m == null ? null : MapToDomain(m);
    }
    public async Task<IEnumerable<Ticket>> GetActiveByCompanyCenAsync(string companyCen)
    {
        var models = await _dbContext.Tickets.Include(t => t.Items).AsNoTracking().Where(t => t.CompanyCen == companyCen && t.Status == "OPEN").ToListAsync();
        return models.Select(m => MapToDomain(m));
    }
    public async Task<int> GetNextDailyNumberAsync(string companyCen)
    {
        var count = await _dbContext.Tickets.CountAsync(t => t.CompanyCen == companyCen && t.CreatedAt.Date == DateTimeOffset.UtcNow.Date);
        return count + 1;
    }
    public async Task AddAsync(Ticket ticket) => await _dbContext.Tickets.AddAsync(MapToModel(ticket));
    public async Task UpdateAsync(Ticket ticket)
    {
        var m = await _dbContext.Tickets.FirstOrDefaultAsync(x => x.TicketCen == ticket.TicketCen);
        if (m != null)
        {
            m.Status = ticket.Status; m.WaiterCen = ticket.WaiterCen;
            m.Subtotal = ticket.Subtotal; m.TaxAmount = ticket.TaxAmount; m.Total = ticket.Total;
            _dbContext.Tickets.Update(m);
        }
    }
    public async Task AddItemAsync(string ticketCen, TicketItem item)
    {
        await _dbContext.TicketItems.AddAsync(new TicketItemModel
        {
            TicketItemCen = item.TicketItemCen,
            TicketCen = ticketCen,
            ProductCen = item.ProductCen,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            Note = item.Note,
            KdsStatus = item.KdsStatus
        });
    }
    public async Task UpdateItemAsync(TicketItem item)
    {
        var m = await _dbContext.TicketItems.FirstOrDefaultAsync(x => x.TicketItemCen == item.TicketItemCen);
        if (m != null)
        {
            m.Quantity = item.Quantity; m.Note = item.Note; m.KdsStatus = item.KdsStatus;
            m.SentAt = item.SentAt; m.ResendCount = item.ResendCount;
            _dbContext.TicketItems.Update(m);
        }
    }
    public async Task<TicketItem?> GetItemByCenAsync(string itemCen)
    {
        var m = await _dbContext.TicketItems.AsNoTracking().FirstOrDefaultAsync(x => x.TicketItemCen == itemCen);
        if (m == null) return null;
        var i = new TicketItem(m.ProductCen, m.Quantity, m.UnitPrice, m.Note);
        typeof(TicketItem).GetProperty(nameof(TicketItem.TicketItemCen))?.SetValue(i, m.TicketItemCen);
        typeof(TicketItem).GetProperty(nameof(TicketItem.KdsStatus))?.SetValue(i, m.KdsStatus);
        typeof(TicketItem).GetProperty(nameof(TicketItem.SentAt))?.SetValue(i, m.SentAt);
        typeof(TicketItem).GetProperty(nameof(TicketItem.ResendCount))?.SetValue(i, m.ResendCount);
        return i;
    }

    private static Ticket MapToDomain(TicketModel m)
    {
        var t = new Ticket(m.CompanyCen, m.DailyNumber, m.WaiterCen);
        typeof(Ticket).GetProperty(nameof(Ticket.TicketCen))?.SetValue(t, m.TicketCen);
        typeof(Ticket).GetProperty(nameof(Ticket.Status))?.SetValue(t, m.Status);
        typeof(Ticket).GetProperty(nameof(Ticket.CreatedAt))?.SetValue(t, m.CreatedAt);
        foreach (var i in m.Items)
        {
            t.AddItem(i.ProductCen, i.Quantity, i.UnitPrice, i.Note);
            var last = t.Items.Last();
            typeof(TicketItem).GetProperty(nameof(TicketItem.TicketItemCen))?.SetValue(last, i.TicketItemCen);
            typeof(TicketItem).GetProperty(nameof(TicketItem.KdsStatus))?.SetValue(last, i.KdsStatus);
            typeof(TicketItem).GetProperty(nameof(TicketItem.SentAt))?.SetValue(last, i.SentAt);
            typeof(TicketItem).GetProperty(nameof(TicketItem.ResendCount))?.SetValue(last, i.ResendCount);
        }
        return t;
    }
    private static TicketModel MapToModel(Ticket t)
    {
        var m = new TicketModel { TicketCen = t.TicketCen, CompanyCen = t.CompanyCen, DailyNumber = t.DailyNumber, WaiterCen = t.WaiterCen, Status = t.Status, Subtotal = t.Subtotal, TaxAmount = t.TaxAmount, Total = t.Total, CreatedAt = t.CreatedAt };
        foreach (var i in t.Items) m.Items.Add(new TicketItemModel { TicketItemCen = i.TicketItemCen, TicketCen = t.TicketCen, ProductCen = i.ProductCen, Quantity = i.Quantity, UnitPrice = i.UnitPrice, Note = i.Note, KdsStatus = i.KdsStatus, SentAt = i.SentAt, ResendCount = i.ResendCount });
        return m;
    }
}

public class SaleRepository : ISaleRepository
{
    private readonly ApplicationDbContext _dbContext;
    public SaleRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public async Task<Sale?> GetByTicketCenAsync(string ticketCen)
    {
        var m = await _dbContext.Sales.AsNoTracking().FirstOrDefaultAsync(x => x.TicketCen == ticketCen);
        return m == null ? null : MapToDomain(m);
    }
    public async Task<IEnumerable<Sale>> GetDailyByCompanyCenAsync(string companyCen, DateTimeOffset date)
    {
        var models = await _dbContext.Sales.AsNoTracking().Where(s => s.CreatedAt.Date == date.Date).ToListAsync();
        return models.Select(m => MapToDomain(m));
    }
    public async Task AddAsync(Sale sale) => await _dbContext.Sales.AddAsync(new SaleModel { SaleCen = sale.SaleCen, TicketCen = sale.TicketCen, PaymentMethodCode = sale.PaymentMethodCode, InventoryDocumentCen = sale.InventoryDocumentCen, Total = sale.Total, CreatedAt = sale.CreatedAt });
    private static Sale MapToDomain(SaleModel m)
    {
        var s = new Sale(m.TicketCen, m.Total, m.PaymentMethodCode, m.InventoryDocumentCen);
        typeof(Sale).GetProperty(nameof(Sale.SaleCen))?.SetValue(s, m.SaleCen);
        typeof(Sale).GetProperty(nameof(Sale.CreatedAt))?.SetValue(s, m.CreatedAt);
        return s;
    }
}

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    public UnitOfWork(ApplicationDbContext dbContext) => _dbContext = dbContext;
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => await _dbContext.SaveChangesAsync(cancellationToken);
}
