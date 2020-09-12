using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StockFundamentalAnalysis.Common.AlphaVantage
{
    //5 API requests per minute and 500 requests per day
    public class Client : IAlphaVantageClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly JsonSerializerSettings _serializingSettings;

        public Client(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
            _serializingSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                Formatting = Formatting.None,
                Converters = new List<JsonConverter> { new DecimalConverter(), new DateTimeConverter() }
            };
        }

        public async Task<StockOverview> GetOverviewAsync(string ticker)
        {
            var responseMessage = await _httpClient.GetAsync($"https://www.alphavantage.co/query?function=OVERVIEW&symbol={ticker}&apikey={_apiKey}");
            responseMessage.EnsureSuccessStatusCode();
            var input = await responseMessage.Content.ReadAsStringAsync();
            var overview = JsonConvert.DeserializeObject<StockOverview>(
                input, _serializingSettings);

            return overview;
        }
    }
}
