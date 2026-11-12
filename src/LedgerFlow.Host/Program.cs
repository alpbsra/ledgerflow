using LedgerFlow.Orders;
using LedgerFlow.Payments;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOrdersModule(builder.Configuration);
builder.Services.AddPaymentsModule(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => "LedgerFlow is running");
app.MapOrdersEndpoints();
app.MapPaymentsEndpoints();

app.Run();