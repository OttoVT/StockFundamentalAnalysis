using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace StockFundamentalAnalysis.Common.AlphaVantage
{
    public class ClientWithRateLimit : IAlphaVantageClient
    {
        private readonly Client _client;
        private readonly IMemoryCache _cache;
        private readonly int _requestsPerMinute;
        private readonly int _requestsPerDay;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public ClientWithRateLimit(
            Client client,
            IMemoryCache cache,
            int requestsPerMinute = 5,
            int requestsPerDay = 500)
        {
            _client = client;
            _cache = cache;
            _requestsPerMinute = requestsPerMinute;
            _requestsPerDay = requestsPerDay;
        }
        public async Task<StockOverview> GetOverviewAsync(string ticker)
        {
            await _semaphore.WaitAsync();

            try
            {
                begin:
                var limitPerMinuteKey = "perMinuteKey";
                var limitPerDayKey = "perDayKey";
                var currentMinuteLimit = await _cache.GetOrCreateAsync<RateLimitCounter>(limitPerMinuteKey, entry =>
                {
                    var counter = new RateLimitCounter()
                    {
                        Counter = 0,
                        UpdatedAt = DateTime.UtcNow,
                        LimitEndAt = DateTime.UtcNow + +TimeSpan.FromMinutes(1)
                    };
                    entry.AbsoluteExpiration = counter.LimitEndAt;
                    entry.Value = counter;

                    return Task.FromResult(counter);
                });

                var currentDayLimit = await _cache.GetOrCreateAsync<RateLimitCounter>(limitPerDayKey, entry =>
                {
                    var counter = new RateLimitCounter();
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
                    entry.Value = counter;

                    return Task.FromResult(counter);
                });

                if (currentMinuteLimit.Counter > _requestsPerMinute)
                {
                    var waitFor= currentMinuteLimit.LimitEndAt - DateTime.UtcNow;
                    await Task.Delay(waitFor);
                    goto begin;
                }

                if (currentDayLimit.Counter > _requestsPerDay)
                {
                    throw new InvalidOperationException("Day limit has been reached.");
                }

                var overview = await _client.GetOverviewAsync(ticker);

                currentMinuteLimit.Counter++;
                currentDayLimit.Counter++;

                return overview;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}