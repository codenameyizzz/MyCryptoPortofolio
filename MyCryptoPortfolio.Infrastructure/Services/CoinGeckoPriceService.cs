using Microsoft.Extensions.Caching.Memory;
using MyCryptoPortfolio.Domain.Interfaces;
using Newtonsoft.Json.Linq;

namespace MyCryptoPortfolio.Infrastructure.Services
{
    public class CoinGeckoPriceService : IPriceService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache; 

        // Daftar Aset yang Didukung (tambah manual sesuai kebutuhan)
        public static readonly Dictionary<string, string> SupportedCoins = new()
        {
            { "BTC", "bitcoin" },
            { "ETH", "ethereum" },
            { "USDT", "tether" },
            { "BNB", "binancecoin" },
            { "SOL", "solana" },
            { "XRP", "ripple" },
            { "USDC", "usd-coin" },
            { "ADA", "cardano" },
            { "AVAX", "avalanche-2" },
            { "DOGE", "dogecoin" },
            { "DOT", "polkadot" },
            { "MATIC", "matic-network" },
            { "SHIB", "shiba-inu" },
            { "LTC", "litecoin" },
            // add more, format: { "TICKER", "id-coingecko" }
        };

        public CoinGeckoPriceService(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        public async Task<decimal> GetPriceAsync(string ticker)
        {
            string cleanTicker = ticker.ToUpper().Trim();
            string cacheKey = $"PRICE_{cleanTicker}"; 

            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                // Simpan harga selama 10 menit. 
                // Selama 10 menit ke depan, tidak tanya ke API lagi.
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

                if (!SupportedCoins.TryGetValue(cleanTicker, out string coinId))
                {
                    return 0m; 
                }

                try
                {
                    _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("MyCryptoPortfolio/1.0");

                    var url = $"https://api.coingecko.com/api/v3/simple/price?ids={coinId}&vs_currencies=idr";
                    var response = await _httpClient.GetStringAsync(url);
                    var json = JObject.Parse(response);

                    var price = json[coinId]?["idr"]?.Value<decimal>();
                    return price ?? 0m;
                }
                catch
                {
                    // jika ika API Gagal (Rate Limit/Internet Mati),
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(2);
                    return 0m; 
                }
            });
        }

        public Dictionary<string, string> GetCoinList()
        {
            return SupportedCoins;
        }
    }
}