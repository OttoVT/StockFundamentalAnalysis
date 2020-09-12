using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StockFundamentalAnalysis.Common.AlphaVantage;

namespace StockFundamentalAnalysis
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var apiKey = Environment.GetEnvironmentVariable("ALPHA_VANTAGE_API_KEY");
            var text = File.ReadAllText("Tickers.json");
            var tickersToTrack = JsonConvert.DeserializeObject<TickerJsonFile>(text);

            var searchPath = AppDomain.CurrentDomain.BaseDirectory;
            var files = Directory.GetFiles(searchPath, "*.json", SearchOption.TopDirectoryOnly)
                .Select(x =>
                {
                    return x.Replace(searchPath, "").Split('.')[0];
                });
            var client = new Client(new HttpClient(), apiKey);
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMemoryCache();
            var sp = serviceCollection.BuildServiceProvider();
            var memCache = sp.GetRequiredService<IMemoryCache>();
            var alphaVantageClient = new ClientWithRateLimit(client, memCache);

            var tickers = tickersToTrack.Tickers.Except(files).ToArray();
            foreach (var ticker in tickers)
            {
                var path = ticker + ".json";
                var overview = await alphaVantageClient.GetOverviewAsync(ticker);
                var serialize = JsonConvert.SerializeObject(overview, Formatting.Indented);
                if (File.Exists(path))
                {
                    File.Delete(path);
                    await Task.Delay(50);
                }

                await File.AppendAllTextAsync(path, serialize);
            }
        }
    }
}
