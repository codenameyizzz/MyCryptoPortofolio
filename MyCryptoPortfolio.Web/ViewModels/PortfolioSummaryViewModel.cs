namespace MyCryptoPortfolio.Web.ViewModels
{
    public class PortfolioSummaryViewModel
    {
        public decimal TotalInvestmentValue { get; set; }

        public List<AssetHoldingViewModel> Holdings { get; set; } = new();
    }

    public class AssetHoldingViewModel
    {
        public string Ticker { get; set; } = string.Empty;
        public decimal TotalQuantity { get; set; }
        public decimal AverageBuyPrice { get; set; } 
        public decimal TotalCostBasis { get; set; } 

        public decimal CurrentMarketPrice { get; set; } 
        public decimal CurrentTotalValue => TotalQuantity * CurrentMarketPrice; 

        public decimal UnrealizedPnL => CurrentTotalValue - TotalCostBasis; 
        
        public decimal PnLPercentage => TotalCostBasis == 0 ? 0 : (UnrealizedPnL / TotalCostBasis) * 100;

        public decimal AllocationPercentage { get; set; }
    }
}