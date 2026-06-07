using prismodSale.Src.Application.DTOs.Sales;
using prismodSale.Src.Application.Interfaces;
using prismodSale.Src.Infraestructure.Persistence.Interfaces;

namespace prismodSale.Src.Application.Services;

public class ConfigService : IConfigService
{
    private readonly ITaxConfigurationRepository _taxRepo;
    private readonly IPaymentMethodRepository _payRepo;
    private readonly IWaiterRepository _waiterRepo;
    private readonly IUnitOfWork _uow;

    public ConfigService(ITaxConfigurationRepository taxRepo, IPaymentMethodRepository payRepo, IWaiterRepository waiterRepo, IUnitOfWork uow)
    {
        _taxRepo = taxRepo;
        _payRepo = payRepo;
        _waiterRepo = waiterRepo;
        _uow = uow;
    }

    public async Task<TaxConfigurationContractResponse> GetTaxConfigurationAsync(string companyCen)
    {
        var config = await _taxRepo.GetByCompanyCenAsync(companyCen);
        return new TaxConfigurationContractResponse { CompanyCen = companyCen, GlobalTaxPercentage = config?.GlobalTaxPercentage ?? 0 };
    }

    public async Task<TaxConfigurationContractResponse> UpdateTaxConfigurationAsync(string companyCen, UpdateTaxConfigurationContractRequest request)
    {
        var config = await _taxRepo.GetByCompanyCenAsync(companyCen);
        if (config == null)
        {
            config = new Domain.Entities.TaxConfiguration(companyCen, request.GlobalTaxPercentage);
            await _taxRepo.AddAsync(config);
        }
        else
        {
            config.UpdatePercentage(request.GlobalTaxPercentage);
            await _taxRepo.UpdateAsync(config);
        }
        await _uow.SaveChangesAsync();
        return new TaxConfigurationContractResponse { CompanyCen = companyCen, GlobalTaxPercentage = config.GlobalTaxPercentage };
    }

    public async Task<IEnumerable<PaymentMethodContractResponse>> GetPaymentMethodsAsync()
    {
        var methods = await _payRepo.GetAllAsync();
        return methods.Select(m => new PaymentMethodContractResponse { PaymentMethodCode = m.PaymentMethodCode, Name = m.Name });
    }

    public async Task<IEnumerable<WaiterContractResponse>> GetWaitersAsync(string companyCen)
    {
        var waiters = await _waiterRepo.GetByCompanyCenAsync(companyCen);
        return waiters.Select(w => new WaiterContractResponse { WaiterCen = w.WaiterCen, Name = w.Name });
    }
}
