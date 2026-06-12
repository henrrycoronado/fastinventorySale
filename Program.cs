using DotNetEnv;
using Polly;
using Polly.Extensions.Http;

using fastinventorySale.Src.Application.Interfaces;
using fastinventorySale.Src.Application.Services;
using fastinventorySale.Src.Infraestructure.ExternalServices;
using fastinventorySale.Src.Infraestructure.Persistence;
using fastinventorySale.Src.Infraestructure.Persistence.Interfaces;
using fastinventorySale.Src.Infraestructure.Persistence.Repositories;
using fastinventorySale.Src.Presentation.MIddleware;

using Microsoft.EntityFrameworkCore;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
var inventoryApiUrl = Environment.GetEnvironmentVariable("INVENTORY_API_URL");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("DB_CONNECTION_STRING is not set.");
}

if (string.IsNullOrEmpty(inventoryApiUrl))
{
    throw new InvalidOperationException("INVENTORY_API_URL is not set.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));


builder.Services.AddScoped<ITaxConfigurationRepository, TaxConfigurationRepository>();
builder.Services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
builder.Services.AddScoped<IWaiterRepository, WaiterRepository>();
builder.Services.AddScoped<IKdsTeamRepository, KdsTeamRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

var circuitBreakerPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

builder.Services.AddHttpClient<IInventoryClient, InventoryClient>(client =>
{
    client.BaseAddress = new Uri(inventoryApiUrl);
})
.AddPolicyHandler(retryPolicy)
.AddPolicyHandler(circuitBreakerPolicy);


builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IKdsService, KdsService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<IConfigService, ConfigService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();