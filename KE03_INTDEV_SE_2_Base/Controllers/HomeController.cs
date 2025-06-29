using System.Diagnostics;
using DataAccessLayer.Interfaces;
using KE03_INTDEV_SE_2_Base.Models;
using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.Models;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    // Controller voor de hoofdpagina en algemene functionaliteiten van de applicatie
    public class HomeController : Controller
    {
        // Logger voor het bijhouden van applicatie events en fouten
        private readonly ILogger<HomeController> _logger;
        
        // Repository voor product-gerelateerde database operaties
        private readonly IProductRepository _productRepository;

        // Constructor voor dependency injection van logger en product repository
        public HomeController(ILogger<HomeController> logger, IProductRepository ProductRepository)
        {
            _logger = logger;
            _productRepository = ProductRepository;
        }

        // Toont de hoofdpagina van de applicatie
        public IActionResult Index()
        {
            return View();
        }

        // Toont de privacy pagina met informatie over gegevensbescherming
        public IActionResult Privacy()
        {
            return View();
        }

        // Toont een foutpagina wanneer er een onverwachte fout optreedt
        // ResponseCache is uitgeschakeld om altijd de meest recente foutinformatie te tonen
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
