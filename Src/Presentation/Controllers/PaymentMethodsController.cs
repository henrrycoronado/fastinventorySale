using fastinventorySale.Src.Application.DTOs.Sales;
using fastinventorySale.Src.Application.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace fastinventorySale.Src.Presentation.Controllers;

[ApiController]
[Route("api/sales/payment-methods")]
public class PaymentMethodsController : ControllerBase
{
    private readonly IConfigService _configService;
    public PaymentMethodsController(IConfigService configService) => _configService = configService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaymentMethodContractResponse>>> GetAll() => Ok(await _configService.GetPaymentMethodsAsync());
}
