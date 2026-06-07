using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using prismodSale.Src.Application.Interfaces;
using prismodSale.Src.Application.Services;
using prismodSale.Src.Infraestructure.Persistence;
using prismodSale.Src.Infraestructure.Persistence.Interfaces;
using prismodSale.Src.Infraestructure.Persistence.Repositories;
using prismodSale.Src.Presentation.MIddleware;
using prismodSale.Src.Infraestructure.ExternalServices;

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

// Repositories
builder.Services.AddScoped<ITaxConfigurationRepository, TaxConfigurationRepository>();
builder.Services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
builder.Services.AddScoped<IWaiterRepository, WaiterRepository>();
builder.Services.AddScoped<IKdsTeamRepository, KdsTeamRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// External Clients
builder.Services.AddHttpClient<IInventoryClient, InventoryClient>(client =>
{
    client.BaseAddress = new Uri(inventoryApiUrl);
});

// Services
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
