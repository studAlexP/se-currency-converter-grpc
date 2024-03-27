using Grpc.Core;
using se_currency_converter_grpc.Helpers;

namespace se_currency_converter_grpc.Services;

public class CurrencyConverterService(ExchangeRateProvider exchangeRateProvider)
    : CurrencyConverter.CurrencyConverterBase
{
    public override async Task<ConvertCurrencyReply> ConvertCurrency(ConvertCurrencyRequest request,
        ServerCallContext context)
    {
        var rate = await exchangeRateProvider.GetExchangeRate(request.FromCurrency, request.ToCurrency);
        var convertedAmount = Math.Round(request.Amount * rate, 2);

        return new ConvertCurrencyReply
        {
            ConvertedAmount = convertedAmount
        };
    }
}