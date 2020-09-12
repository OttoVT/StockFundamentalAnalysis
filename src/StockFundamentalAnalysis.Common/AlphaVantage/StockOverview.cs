using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StockFundamentalAnalysis.Common.AlphaVantage
{
    public class StockOverview
    {
        public string Symbol { get; set; }
        public string AssetType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Exchange { get; set; }
        public string Currency { get; set; }
        public string Country { get; set; }
        public string Sector { get; set; }
        public string Industry { get; set; }
        public string Address { get; set; }
        public long FullTimeEmployees { get; set; }
        public string FiscalYearEnd { get; set; }
        public DateTime LatestQuarter { get; set; }

        public decimal MarketCapitalization { get; set; }
        public decimal EBITDA { get; set; }
        public decimal PERatio { get; set; }
        public decimal PEGRatio { get; set; }
        public decimal BookValue { get; set; }
        public decimal DividendPerShare { get; set; }
        public decimal DividendYield { get; set; }
        public decimal EPS { get; set; }
        public decimal RevenuePerShareTTM { get; set; }
        public decimal ProfitMargin { get; set; }
        public decimal OperatingMarginTTM { get; set; }
        public decimal ReturnOnAssetsTTM { get; set; }
        public decimal ReturnOnEquityTTM { get; set; }
        public decimal RevenueTTM { get; set; }
        public decimal GrossProfitTTM { get; set; }
        public decimal DilutedEPSTTM { get; set; }
        public decimal QuarterlyEarningsGrowthYOY { get; set; }
        public decimal QuarterlyRevenueGrowthYOY { get; set; }
        public decimal AnalystTargetPrice { get; set; }
        public decimal TrailingPE { get; set; }
        public decimal ForwardPE { get; set; }
        public decimal PriceToSalesRatioTTM { get; set; }
        public decimal PriceToBookRatio { get; set; }
        public decimal EVToRevenue { get; set; }
        public decimal EVToEBITDA { get; set; }
        public decimal Beta { get; set; }

        [JsonProperty(PropertyName = "52WeekHigh")]
        public decimal WeekHigh52 { get; set; }

        [JsonProperty(PropertyName = "52WeekLow")]
        public decimal WeekLow52 { get; set; }

        [JsonProperty(PropertyName = "50DayMovingAverage")]
        public decimal DayMovingAverage50 { get; set; }

        [JsonProperty(PropertyName = "200DayMovingAverage")]
        public decimal DayMovingAverage200 { get; set; }
        public decimal SharesOutstanding { get; set; }
        public decimal SharesFloat { get; set; }
        public decimal SharesShort { get; set; }
        public decimal SharesShortPriorMonth { get; set; }
        public decimal ShortRatio { get; set; }
        public decimal ShortPercentOutstanding { get; set; }
        public decimal ShortPercentFloat { get; set; }
        public decimal PercentInsiders { get; set; }
        public decimal PercentInstitutions { get; set; }
        public decimal ForwardAnnualDividendRate { get; set; }
        public decimal ForwardAnnualDividendYield { get; set; }
        public decimal PayoutRatio { get; set; }
        public DateTime DividendDate { get; set; }
        public DateTime ExDividendDate { get; set; }
        public string LastSplitFactor { get; set; }
        public DateTime LastSplitDate { get; set; }
    }

    internal class DecimalConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(decimal) || objectType == typeof(decimal?));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Float || token.Type == JTokenType.Integer)
            {
                return token.ToObject<decimal>();
            }
            if (token.Type == JTokenType.String)
            {
                if (token.ToString() == "None")
                    return 0m;
                // customize this to suit your needs
                return Decimal.Parse(token.ToString(),
                    System.Globalization.CultureInfo.InvariantCulture);
            }
            if (token.Type == JTokenType.Null && objectType == typeof(decimal?))
            {
                return null;
            }
            throw new JsonSerializationException("Unexpected token type: " +
                                                 token.Type.ToString());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    internal class DateTimeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(DateTime) || objectType == typeof(DateTime?));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Date)
            {
                return token.ToObject<DateTime>();
            }
            if (token.Type == JTokenType.String)
            {
                if (token.ToString() == "None")
                    return DateTime.MinValue;
                // customize this to suit your needs
                return DateTime.Parse(token.ToString(),
                    System.Globalization.CultureInfo.InvariantCulture);
            }
            if (token.Type == JTokenType.Null && objectType == typeof(DateTime?))
            {
                return null;
            }
            throw new JsonSerializationException("Unexpected token type: " +
                                                 token.Type.ToString());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
