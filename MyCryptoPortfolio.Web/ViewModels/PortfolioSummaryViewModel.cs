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
        public decimal AverageBuyPrice { get; set; } // Harga Beli Rata-rata
        public decimal TotalCostBasis { get; set; } // Total Modal Keluar

        // --- TAMBAHAN BARU ---
        public decimal CurrentMarketPrice { get; set; } // Harga Pasar Live
        public decimal CurrentTotalValue => TotalQuantity * CurrentMarketPrice; // Nilai Aset Sekarang
        
        // Menghitung Profit/Loss dalam Rupiah/Dollar
        public decimal UnrealizedPnL => CurrentTotalValue - TotalCostBasis; 
        
        // Menghitung Persentase Profit/Loss
        public decimal PnLPercentage => TotalCostBasis == 0 ? 0 : (UnrealizedPnL / TotalCostBasis) * 100;

        // --- TAMBAHAN BARU: Persentase Alokasi ---
        public decimal AllocationPercentage { get; set; }
    }
}