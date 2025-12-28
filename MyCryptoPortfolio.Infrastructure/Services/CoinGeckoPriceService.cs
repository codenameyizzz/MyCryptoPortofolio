using Microsoft.Extensions.Caching.Memory;
using MyCryptoPortfolio.Domain.Interfaces;
using Newtonsoft.Json.Linq;

namespace MyCryptoPortfolio.Infrastructure.Services
{
    public class CoinGeckoPriceService : IPriceService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache; // Memori penyimpanan sementara

        // Daftar Aset yang Didukung (Bisa ditambah manual)
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
            // Tambahkan koin lain di sini, format: { "TICKER", "id-coingecko" }
        };

        public CoinGeckoPriceService(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        public async Task<decimal> GetPriceAsync(string ticker)
        {
            string cleanTicker = ticker.ToUpper().Trim();
            string cacheKey = $"PRICE_{cleanTicker}"; // Kunci memori, misal "PRICE_BTC"

            // Cek Memori dulu (Cache)
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                // ATURAN 1: Simpan harga selama 10 menit. 
                // Selama 10 menit ke depan, aplikasi TIDAK AKAN tanya ke API lagi.
                // Ini membuat grafik STABIL dan loading INSTAN.
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

                if (!SupportedCoins.TryGetValue(cleanTicker, out string coinId))
                {
                    return 0m; // Aset tidak dikenal
                }

                try
                {
                    // Tambahkan User-Agent agar tidak dianggap bot spammer
                    _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("MyCryptoPortfolio/1.0");

                    var url = $"https://api.coingecko.com/api/v3/simple/price?ids={coinId}&vs_currencies=idr";
                    var response = await _httpClient.GetStringAsync(url);
                    var json = JObject.Parse(response);

                    // Ambil harga
                    var price = json[coinId]?["idr"]?.Value<decimal>();
                    return price ?? 0m;
                }
                catch
                {
                    // ATURAN 2: Jika API Gagal (Rate Limit/Internet Mati),
                    // Jangan simpan nilai 0 ini lama-lama. Coba lagi dalam 2 detik.
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