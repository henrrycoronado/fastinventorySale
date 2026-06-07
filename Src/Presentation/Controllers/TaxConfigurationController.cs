using fastinventorySale.Src.Application.DTOs.Sales;
using fastinventorySale.Src.Application.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace fastinventorySale.Src.Presentation.Controllers;

[ApiController]
[Route("api/sales/companies/{companyCen}/tax-configuration")]
public class TaxConfigurationController : ControllerBase
{
    private readonly IConfigService _configService;
    public TaxConfigurationController(IConfigService configService) => _configService = configService;

    [HttpGet]
    public async Task<ActionResult<TaxConfigurationContractResponse>> Get(string companyCen) => Ok(await _configService.GetTaxConfigurationAsync(companyCen));

    [HttpPut]
    public async Task<ActionResult<TaxConfigurationContractResponse>> Update(string companyCen, [FromBody] UpdateTaxConfigurationContractRequest request) => Ok(await _configService.UpdateTaxConfigurationAsync(companyCen, request));
}
