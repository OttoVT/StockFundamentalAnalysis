using System.Threading.Tasks;

namespace StockFundamentalAnalysis.Common.AlphaVantage
{
    public interface IAlphaVantageClient
    {
        Task<StockOverview> GetOverviewAsync(string ticker);
    }
}