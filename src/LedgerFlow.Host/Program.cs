using LedgerFlow.Orders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOrdersModule();

var app = builder.Build();

app.MapGet("/", () => "LedgerFlow is running");
app.MapOrdersEndpoints();

app.Run();