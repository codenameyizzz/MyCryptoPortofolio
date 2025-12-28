using Microsoft.AspNetCore.Mvc;

namespace MyCryptoPortfolio.Web.Controllers
{
    public class MarketController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}