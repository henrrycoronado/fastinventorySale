using Microsoft.AspNetCore.Mvc;
using prismodSale.Src.Application.DTOs.Sales;
using prismodSale.Src.Application.Interfaces;

namespace prismodSale.Src.Presentation.Controllers;

[ApiController]
[Route("api/sales/payment-methods")]
public class PaymentMethodsController : ControllerBase
{
    private readonly IConfigService _configService;
    public PaymentMethodsController(IConfigService configService) => _configService = configService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaymentMethodContractResponse>>> GetAll() => Ok(await _configService.GetPaymentMethodsAsync());
}
