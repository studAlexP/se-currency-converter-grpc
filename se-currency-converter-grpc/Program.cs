using se_currency_converter_grpc.Helpers;
using se_currency_converter_grpc.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<ExchangeRateProvider>(sp =>
{
    var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var client = clientFactory.CreateClient();
    return new ExchangeRateProvider(client);
});

var app = builder.Build();

app.MapGrpcService<CurrencyConverterService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

IWebHostEnvironment env = app.Environment;

if (env.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.Run();