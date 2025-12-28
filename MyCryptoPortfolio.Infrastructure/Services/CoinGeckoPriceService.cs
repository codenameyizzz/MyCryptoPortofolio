using MyCryptoPortfolio.Domain.Interfaces;
using Newtonsoft.Json.Linq;

namespace MyCryptoPortfolio.Infrastructure.Services
{
    public class CoinGeckoPriceService : IPriceService
    {
        private readonly HttpClient _httpClient;

        public CoinGeckoPriceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetPriceAsync(string ticker)
        {
            // 1. Mapping Ticker ke ID CoinGecko
            var coinId = GetCoinId(ticker);
            if (string.IsNullOrEmpty(coinId)) return 0; // Jika koin tidak dikenal, anggap harga 0

            try
            {
                // 2. Panggil API CoinGecko
                var response = await _httpClient.GetStringAsync($"https://api.coingecko.com/api/v3/simple/price?ids={coinId}&vs_currencies=usd");
                
                // 3. Baca JSON hasilnya
                var json = JObject.Parse(response);
                var price = json[coinId]?["usd"]?.Value<decimal>();

                return price ?? 0;
            }
            catch
            {
                return 0;
            }
        }

        private string GetCoinId(string ticker)
        {
            return ticker.ToLower() switch
            {
                "btc" => "bitcoin",
                "eth" => "ethereum",
                "sol" => "solana",
                "bnb" => "binancecoin",
                "xrp" => "ripple",
                "doge" => "dogecoin",
                "ada" => "cardano",
                _ => "" 
            };
        }
    }
}