namespace MyCryptoPortfolio.Domain.Interfaces
{
    public interface IPriceService
    {
        Task<decimal> GetPriceAsync(string ticker);
        Dictionary<string, string> GetCoinList();
    }
}