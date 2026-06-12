using fastinventorySale.Src.Domain.Entities;

namespace fastinventorySale.Src.Infraestructure.Persistence.Interfaces;

public interface ITaxConfigurationRepository
{
    Task<TaxConfiguration?> GetByCompanyCenAsync(string companyCen);
    Task AddAsync(TaxConfiguration config);
    Task UpdateAsync(TaxConfiguration config);
}

public interface IPaymentMethodRepository
{
    Task<IEnumerable<PaymentMethod>> GetAllAsync();
    Task<PaymentMethod?> GetByCodeAsync(string code);
}

public interface IWaiterRepository
{
    Task<IEnumerable<Waiter>> GetByCompanyCenAsync(string companyCen);
    Task<Waiter?> GetByCenAsync(string waiterCen);
    Task AddAsync(Waiter waiter);
}

public interface IKdsTeamRepository
{
    Task<IEnumerable<KdsTeam>> GetByCompanyCenAsync(string companyCen);
    Task<KdsTeam?> GetByCenAsync(string teamCen);
    Task AddAsync(KdsTeam team);
}

public interface ITicketRepository
{
    Task<Ticket?> GetByCenAsync(string ticketCen);
    Task<IEnumerable<Ticket>> GetActiveByCompanyCenAsync(string companyCen);
    Task<int> GetNextDailyNumberAsync(string companyCen);
    Task AddAsync(Ticket ticket);
    Task UpdateAsync(Ticket ticket);
    Task AddItemAsync(string ticketCen, TicketItem item);
    Task UpdateItemAsync(TicketItem item);
    Task<TicketItem?> GetItemByCenAsync(string itemCen);
}

public interface ISaleRepository
{
    Task<Sale?> GetByTicketCenAsync(string ticketCen);
    Task<IEnumerable<Sale>> GetDailyByCompanyCenAsync(string companyCen, DateTimeOffset date);
    Task<IEnumerable<Sale>> GetByMonthlyRangeAsync(string companyCen, DateTimeOffset start, DateTimeOffset end);
    Task AddAsync(Sale sale);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}