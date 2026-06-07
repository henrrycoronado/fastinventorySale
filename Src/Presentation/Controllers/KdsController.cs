using fastinventorySale.Src.Application.DTOs.Sales;
using fastinventorySale.Src.Application.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace fastinventorySale.Src.Presentation.Controllers;

[ApiController]
[Route("api/sales/companies/{companyCen}/kds")]
public class KdsController : ControllerBase
{
    private readonly IKdsService _kdsService;
    public KdsController(IKdsService kdsService) => _kdsService = kdsService;

    [HttpGet("teams")]
    public async Task<ActionResult<IEnumerable<KdsTeamContractResponse>>> GetTeams(string companyCen) => Ok(await _kdsService.GetTeamsAsync(companyCen));

    [HttpPost("teams")]
    public async Task<ActionResult<KdsTeamContractResponse>> CreateTeam(string companyCen, [FromBody] CreateKdsTeamDto dto)
    {
        dto.CompanyCen = companyCen;
        var team = await _kdsService.CreateTeamAsync(dto);
        return CreatedAtAction(nameof(GetTeams), new { companyCen }, team);
    }

    [HttpGet("teams/{teamCen}/items")]
    public async Task<ActionResult<IEnumerable<KdsItemContractResponse>>> GetItemsByTeam(string companyCen, string teamCen) => Ok(await _kdsService.GetItemsByTeamAsync(companyCen, teamCen));

    [HttpPatch("items/{ticketItemCen}/status")]
    public async Task<IActionResult> UpdateItemStatus(string companyCen, string ticketItemCen, [FromBody] UpdateKdsItemStatusContractRequest request)
    {
        await _kdsService.UpdateItemStatusAsync(companyCen, ticketItemCen, request.Status);
        return Ok();
    }
}
