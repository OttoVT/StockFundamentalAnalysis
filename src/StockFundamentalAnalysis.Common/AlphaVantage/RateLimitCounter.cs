using System;

namespace StockFundamentalAnalysis.Common.AlphaVantage
{
    public class RateLimitCounter
    {
        public int Counter { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime LimitEndAt { get; set; }
    }
}