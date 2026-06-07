using fastinventorySale.Src.Application.DTOs.Sales;
using fastinventorySale.Src.Application.Interfaces;
using fastinventorySale.Src.Domain.Entities;
using fastinventorySale.Src.Infraestructure.Persistence.Interfaces;

namespace fastinventorySale.Src.Application.Services;

public class KdsService : IKdsService
{
    private readonly IKdsTeamRepository _teamRepo;
    private readonly ITicketRepository _ticketRepo;
    private readonly IUnitOfWork _uow;

    public KdsService(IKdsTeamRepository teamRepo, ITicketRepository ticketRepo, IUnitOfWork uow)
    {
        _teamRepo = teamRepo;
        _ticketRepo = ticketRepo;
        _uow = uow;
    }

    public async Task<IEnumerable<KdsTeamContractResponse>> GetTeamsAsync(string companyCen)
    {
        var teams = await _teamRepo.GetByCompanyCenAsync(companyCen);
        return teams.Select(t => new KdsTeamContractResponse { TeamCen = t.TeamCen, Name = t.Name, CategoryCens = t.CategoryCens.ToList() });
    }

    public async Task<KdsTeamContractResponse> CreateTeamAsync(CreateKdsTeamDto dto)
    {
        var team = new KdsTeam(dto.CompanyCen, dto.Name);
        foreach (var cat in dto.CategoryCens) team.AddCategory(cat);
        await _teamRepo.AddAsync(team);
        await _uow.SaveChangesAsync();
        return new KdsTeamContractResponse { TeamCen = team.TeamCen, Name = team.Name, CategoryCens = team.CategoryCens.ToList() };
    }

    public async Task<IEnumerable<KdsItemContractResponse>> GetItemsByTeamAsync(string companyCen, string teamCen)
    {
        var team = await _teamRepo.GetByCenAsync(teamCen);
        if (team == null) return Enumerable.Empty<KdsItemContractResponse>();

        var tickets = await _ticketRepo.GetActiveByCompanyCenAsync(companyCen);
        var items = tickets.SelectMany(t => t.Items.Select(i => new { Ticket = t, Item = i }))
            .Where(x => x.Item.KdsStatus != "DELIVERED" && x.Item.KdsStatus != "CANCELED")
            .Select(x => new KdsItemContractResponse
            {
                TicketItemCen = x.Item.TicketItemCen,
                TicketCen = x.Ticket.TicketCen,
                ProductName = "Unknown",
                Quantity = x.Item.Quantity,
                Status = x.Item.KdsStatus,
                OrderedAt = x.Ticket.CreatedAt,
                Note = x.Item.Note
            });

        return items;
    }

    public async Task UpdateItemStatusAsync(string companyCen, string ticketItemCen, string status)
    {
        var item = await _ticketRepo.GetItemByCenAsync(ticketItemCen);
        if (item != null)
        {
            item.SetKdsStatus(status);
            await _ticketRepo.UpdateItemAsync(item);
            await _uow.SaveChangesAsync();
        }
    }
}
