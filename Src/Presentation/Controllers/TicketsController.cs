using fastinventorySale.Src.Application.DTOs.Sales;
using fastinventorySale.Src.Application.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace fastinventorySale.Src.Presentation.Controllers;

[ApiController]
[Route("api/sales/companies/{companyCen}/tickets")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly ISaleService _saleService;

    public TicketsController(ITicketService ticketService, ISaleService saleService)
    {
        _ticketService = ticketService;
        _saleService = saleService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TicketContractResponse>>> GetAll(string companyCen) => Ok(await _ticketService.GetActiveByCompanyAsync(companyCen));

    [HttpPost]
    public async Task<ActionResult<TicketContractResponse>> Create(string companyCen, [FromBody] CreateTicketContractRequest request)
    {
        var result = await _ticketService.CreateAsync(companyCen, request);
        return CreatedAtAction(nameof(GetAll), new { companyCen }, result);
    }

    [HttpGet("{ticketCen}/items")]
    public async Task<ActionResult<IEnumerable<TicketItemContractResponse>>> GetItems(string companyCen, string ticketCen) => Ok(await _ticketService.GetItemsAsync(companyCen, ticketCen));

    [HttpPost("{ticketCen}/items")]
    public async Task<ActionResult<TicketItemContractResponse>> AddItem(string companyCen, string ticketCen, [FromBody] CreateTicketItemContractRequest request)
    {
        var result = await _ticketService.AddItemAsync(companyCen, ticketCen, request);
        return CreatedAtAction(nameof(GetItems), new { companyCen, ticketCen }, result);
    }

    [HttpPatch("{ticketCen}/items/{ticketItemCen}")]
    public async Task<ActionResult<TicketItemContractResponse>> UpdateItem(string companyCen, string ticketCen, string ticketItemCen, [FromBody] UpdateTicketItemContractRequest request) => Ok(await _ticketService.UpdateItemAsync(companyCen, ticketCen, ticketItemCen, request));

    [HttpPost("{ticketCen}/send")]
    public async Task<ActionResult<IEnumerable<TicketItemContractResponse>>> SendToKds(string companyCen, string ticketCen)
    {
        var items = await _ticketService.MarkAsSentToKdsAsync(companyCen, ticketCen);
        return Ok(items);
    }

    [HttpPost("{ticketCen}/items/{ticketItemCen}/resend")]
    public async Task<ActionResult<TicketItemContractResponse>> ResendItem(string companyCen, string ticketCen, string ticketItemCen)
    {
        var item = await _ticketService.ResendToKdsAsync(companyCen, ticketCen, ticketItemCen);
        return Ok(item);
    }

    [HttpGet("{ticketCen}/print")]
    public async Task<IActionResult> Print(string companyCen, string ticketCen)
    {
        var content = await _ticketService.PrintTicketAsync(companyCen, ticketCen);
        return File(content, "application/pdf", $"ticket_{ticketCen}.pdf");
    }

    [HttpPut("{ticketCen}/waiter")]
    public async Task<ActionResult<AssignTicketWaiterContractResponse>> AssignWaiter(string companyCen, string ticketCen, [FromBody] AssignTicketWaiterContractRequest request) => Ok(await _ticketService.AssignWaiterAsync(companyCen, ticketCen, request));

    [HttpPost("{ticketCen}/cancel")]
    public async Task<ActionResult<CancelTicketContractResponse>> Cancel(string companyCen, string ticketCen, [FromBody] CancelTicketContractRequest request) => Ok(await _ticketService.CancelAsync(companyCen, ticketCen, request));

    [HttpGet("{ticketCen}/totals")]
    public async Task<ActionResult<TicketTotalsContractResponse>> GetTotals(string companyCen, string ticketCen) => Ok(await _ticketService.GetTotalsAsync(companyCen, ticketCen));

    [HttpPost("{ticketCen}/payment")]
    public async Task<ActionResult<PayTicketContractResponse>> Pay(string companyCen, string ticketCen, [FromBody] PayTicketContractRequest request) => Ok(await _saleService.ProcessPaymentAsync(companyCen, ticketCen, request));
}