using Microsoft.AspNetCore.Mvc;
using prismodSale.Src.Application.DTOs.Sales;
using prismodSale.Src.Application.Interfaces;

namespace prismodSale.Src.Presentation.Controllers;

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
