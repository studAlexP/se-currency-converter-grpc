using System.Net;
using System.Xml.Linq;

namespace se_currency_converter_grpc.Helpers;

public class ExchangeRateProvider(HttpClient httpClient)
{
    public async Task<double> GetExchangeRate(string fromCurrency, string toCurrency)
    {
        const string url = "http://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

        string xmlString = await httpClient.GetStringAsync(url);

        XDocument xmlDoc = XDocument.Parse(xmlString);

        XNamespace ns = "http://www.ecb.int/vocabulary/2002-08-01/eurofxref";

        var fromRate = fromCurrency == "EUR"
            ? 1
            : (double?)xmlDoc.Descendants(ns + "Cube")
                .Where(x => x.Attribute("currency")?.Value == fromCurrency)
                .Select(x => (double)(x.Attribute("rate") ?? throw new InvalidOperationException()))
                .SingleOrDefault();

        var toRate = toCurrency == "EUR"
            ? 1
            : (double?)xmlDoc.Descendants(ns + "Cube")
                .Where(x => x.Attribute("currency")?.Value == toCurrency)
                .Select(x => (double)(x.Attribute("rate") ?? throw new InvalidOperationException()))
                .SingleOrDefault();

        if (fromRate == null || toRate == null)
        {
            throw new Exception("Currency not found.");
        }

        return (double)toRate / (double)fromRate;
    }
}
