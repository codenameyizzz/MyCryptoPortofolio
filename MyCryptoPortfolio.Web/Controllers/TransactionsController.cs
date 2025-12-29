using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyCryptoPortfolio.Domain.Entities;
using MyCryptoPortfolio.Infrastructure.Data;
using MyCryptoPortfolio.Web.ViewModels;
using MyCryptoPortfolio.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace MyCryptoPortfolio.Web.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPriceService _priceService;
        public TransactionsController(ApplicationDbContext context, IPriceService priceService)
        {
            _context = context;
            _priceService = priceService; 
        }

        public async Task<IActionResult> Index()
        {
            var transactions = _context.Transactions.ToList();

            var holdingsList = transactions
                .GroupBy(t => t.Ticker)
                .Select(g => new AssetHoldingViewModel
                {
                    Ticker = g.Key,
                    TotalQuantity = g.Sum(t => t.Type == TransactionType.Buy ? t.Quantity : -t.Quantity),
                    TotalCostBasis = g.Sum(t => t.Type == TransactionType.Buy
                        ? (t.Price * t.Quantity) + t.Fee  
                        : -((t.Price * t.Quantity) - t.Fee)
                    )
                })
                .Where(h => h.TotalQuantity > 0)
                .ToList();

            foreach (var asset in holdingsList)
            {
                asset.AverageBuyPrice = asset.TotalCostBasis / asset.TotalQuantity;
                asset.CurrentMarketPrice = await _priceService.GetPriceAsync(asset.Ticker);
            }

            // hitung total nilai portofolio saat ini (Current Value)
            decimal currentPortfolioValue = holdingsList.Sum(h => h.CurrentTotalValue);

            // persentase untuk setiap aset
            foreach (var asset in holdingsList)
            {
                if (currentPortfolioValue > 0)
                {
                    asset.AllocationPercentage = (asset.CurrentTotalValue / currentPortfolioValue) * 100;
                }
                else
                {
                    asset.AllocationPercentage = 0;
                }
            }

            var viewModel = new PortfolioSummaryViewModel
            {
                TotalInvestmentValue = holdingsList.Sum(h => h.TotalCostBasis),

                Holdings = holdingsList
            };

            ViewBag.CurrentPortfolioValue = holdingsList.Sum(h => h.CurrentTotalValue);

            ViewBag.History = transactions.OrderByDescending(t => t.Date).ToList();

            return View(viewModel);
        }

        public IActionResult History(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Transactions.AsQueryable();

            if (startDate.HasValue)
            {
                // set to jam 00:00:00 
                var start = DateTime.SpecifyKind(startDate.Value.Date, DateTimeKind.Utc);
                query = query.Where(t => t.Date >= start);
            }

            if (endDate.HasValue)
            {
                var end = DateTime.SpecifyKind(endDate.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);
                query = query.Where(t => t.Date <= end);
            }

            var transactions = query.OrderByDescending(t => t.Date).ToList();

            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            return View(transactions);
        }

        // export to csv
        public IActionResult ExportToCsv(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Transactions.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(t => t.Date >= DateTime.SpecifyKind(startDate.Value.Date, DateTimeKind.Utc));
            
            if (endDate.HasValue)
                query = query.Where(t => t.Date <= DateTime.SpecifyKind(endDate.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc));

            var transactions = query.OrderByDescending(t => t.Date).ToList();

            var builder = new System.Text.StringBuilder();
            builder.AppendLine("Date,Ticker,Type,Price,Quantity,Fee,Total Amount"); // Header

            foreach (var t in transactions)
            {
                // csv format: "2023-12-01,BTC,Buy,500000,1,0,500000"
                builder.AppendLine($"{t.Date:yyyy-MM-dd HH:mm},{t.Ticker},{t.Type},{t.Price},{t.Quantity},{t.Fee},{t.TotalAmount}");
            }

            return File(System.Text.Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "TransactionHistory.csv");
        }

        // GET: /Transactions/Create
        public IActionResult Create()
        {
            ViewBag.CoinList = _priceService.GetCoinList(); 

            var viewModel = new TransactionViewModel
            {
                Date = DateTime.Today,
                TypeOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Buy", Text = "Buy (Beli)" },
                    new SelectListItem { Value = "Sell", Text = "Sell (Jual)" }
                }
            };
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetLatestPrice(string ticker)
        {
            if (string.IsNullOrWhiteSpace(ticker)) return Json(0);

            // Panggil Service CoinGecko yang sudah kita buat (sudah ada Cache-nya juga)
            var price = await _priceService.GetPriceAsync(ticker);
            
            return Json(price);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TransactionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var transactionEntity = new Transaction
                {
                    Ticker = model.Ticker.ToUpper(),
                    Price = model.Price,
                    Quantity = model.Quantity,
                    Fee = model.Fee,
                    Date = DateTime.SpecifyKind(model.Date, DateTimeKind.Utc),
                    Type = Enum.Parse<TransactionType>(model.TransactionType)
                };

                _context.Transactions.Add(transactionEntity);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            model.TypeOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "Buy", Text = "Buy (Beli)" },
                new SelectListItem { Value = "Sell", Text = "Sell (Jual)" }
            };
            return View(model);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var transaction = _context.Transactions.Find(id);
            if (transaction == null) return NotFound();

            var viewModel = new TransactionViewModel
            {
                Id = transaction.Id,
                Ticker = transaction.Ticker,
                Quantity = transaction.Quantity,
                Price = transaction.Price,
                Date = transaction.Date,
                TransactionType = transaction.Type.ToString(),
                TypeOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Buy", Text = "Buy (Beli)" },
                    new SelectListItem { Value = "Sell", Text = "Sell (Jual)" }
                }
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, TransactionViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var transaction = _context.Transactions.Find(id);
                if (transaction == null) return NotFound();

                transaction.Ticker = model.Ticker.ToUpper();
                transaction.Quantity = model.Quantity;
                transaction.Price = model.Price;
                transaction.Date = DateTime.SpecifyKind(model.Date, DateTimeKind.Utc);
                transaction.Type = Enum.Parse<TransactionType>(model.TransactionType);

                _context.Update(transaction);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            model.TypeOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "Buy", Text = "Buy (Beli)" },
                new SelectListItem { Value = "Sell", Text = "Sell (Jual)" }
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var transaction = _context.Transactions.Find(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}