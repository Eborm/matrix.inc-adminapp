using System.Diagnostics;
using DataAccessLayer.Interfaces;
using KE03_INTDEV_SE_2_Base.Models;
using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.Models;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    public class HomeController : Controller
    {
            private readonly ILogger<HomeController> _logger;
            private readonly IProductRepository _productRepository;

        public HomeController(ILogger<HomeController> logger, IProductRepository ProductRepository)
        {
            _logger = logger;
            _productRepository = ProductRepository;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = _productRepository.GetAllProducts();
            foreach (var product in products)
            {
                if ((product.DiscountStartTime + product.DiscountDuration) < DateTime.Now.Second)
                {
                    product.DiscountDuration = 0;
                    product.Discount = 0;
                    product.DiscountStartTime = 0;
                    _productRepository.UpdateProduct(product);
                }
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
