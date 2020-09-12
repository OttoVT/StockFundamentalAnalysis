using System;
using System.Collections.Generic;
using System.Text;

namespace StockFundamentalAnalysis.Common.Domain
{
    public class Stock
    {
        private Stock(string ticker)
        {
            Ticker = ticker;
        }

        public string Ticker { get; }

        public static Stock Create(string ticker)
        {
            return new Stock(ticker);
        }

        public static Stock Restore(string ticker)
        {
            return new Stock(ticker);
        }
    }
}
