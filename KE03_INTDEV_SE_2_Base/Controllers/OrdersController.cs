using DataAccessLayer;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using KE03_INTDEV_SE_2_Base.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    // Controller voor het beheren van orders in het systeem
    public class OrdersController : Controller
    {
        // Database context voor directe database toegang
        private readonly MatrixIncDbContext _context;
        
        // Repository voor gebruikers-gerelateerde operaties
        private readonly IUserRepository _UserRepository;
        
        // Controleert of de huidige gebruiker de vereiste permissies heeft
        // requiredPermission: 0 = admin, 1 = manager, 2 = gebruiker
        private bool UserHasPermission(int requiredPermission)
        {
            int userId = HttpContext.Session.GetObjectFromJson<int>("User_id");
            if (userId == 0) return false;
            var user = _UserRepository.GetUserByUID(userId);
            return user != null && user.Permissions <= requiredPermission;
        }
        
        // Constructor voor dependency injection van database context en user repository
        public OrdersController(MatrixIncDbContext context, IUserRepository userRepository)
        {
            _context = context;
            _UserRepository = userRepository;
        }

        // GET: Toont een overzicht van alle orders (alleen voor gebruikers met permissie niveau 2)
        public async Task<IActionResult> Index()
        {
            int userId = HttpContext.Session.GetObjectFromJson<int>("User_id");
            if (userId == 0)
                return Redirect("/User/Login");
            if (!UserHasPermission(2))
            {
                TempData["ErrorMessage"] = "You do not have permission to access that page.";
                return RedirectToAction("Index", "Home");
            }
            // Haal alle orders op met gerelateerde klant, gebruiker en orderregels inclusief producten
            var orders = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.User)
                .Include(o => o.OrderRegels).ThenInclude(or => or.Product)
                .ToList();
            return View(orders);
        }

        // GET: Toont gedetailleerde informatie over een specifieke order (alleen voor gebruikers met permissie niveau 2)
        public async Task<IActionResult> Details(int? id)
        {
            if (!UserHasPermission(2))
            {
                TempData["ErrorMessage"] = "You do not have permission to access that page.";
                return RedirectToAction("Index", "Home");
            }
            int userId = HttpContext.Session.GetObjectFromJson<int>("User_id");
            if (userId == 0)
                return Redirect("/User/Login");
            if (id == null)
                return NotFound();
            // Haal de order op met alle gerelateerde gegevens
            var order = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.User)
                .Include(o => o.OrderRegels).ThenInclude(or => or.Product)
                .FirstOrDefault(o => o.Id == id);
            if (order == null)
                return NotFound();
            return View(order);
        }

        // GET: Toont de pagina voor het aanmaken van een nieuwe order (alleen voor gebruikers met permissie niveau 2)
        public IActionResult Aanmaken()
        {
            if (!UserHasPermission(2))
            {
                TempData["ErrorMessage"] = "You do not have permission to access that page.";
                return RedirectToAction("Index", "Home");
            }
            int userId = HttpContext.Session.GetObjectFromJson<int>("User_id");
            if (userId == 0)
                return Redirect("/User/Login");
            
            // Haal alle producten op voor de order
            var producten = _context.Products.ToList();
            var vm = new OrderAanmakenViewModel
            {
                OrderDate = DateTime.Today,
                Producten = producten.Select(p => new ProductAantal
                {
                    ProductId = p.Id,
                    ProductNaam = p.Name,
                    Aantal = 0
                }).ToList()
            };
            
            // Vul de dropdown lijsten voor gebruikers en klanten
            ViewBag.GebruikersList = new SelectList(_context.Users.ToList(), "Id", "UserName");
            ViewBag.KlantenList = new SelectList(_context.Customers.ToList(), "Id", "Name");
            return View(vm);
        }

        // POST: Verwerkt het aanmaken van een nieuwe order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Aanmaken(OrderAanmakenViewModel vm)
        {
            if (!UserHasPermission(2))
            {
                TempData["ErrorMessage"] = "You do not have permission to access that page.";
                return RedirectToAction("Index", "Home");
            }
            int userId = HttpContext.Session.GetObjectFromJson<int>("User_id");
            if (userId == 0)
                return Redirect("/User/Login");
            if (ModelState.IsValid)
            {
                // Maak een nieuwe order aan
                var order = new Order
                {
                    OrderDate = vm.OrderDate,
                    CustomerId = vm.CustomerId,
                    UserId = vm.UserId
                };
                
                // Voeg orderregels toe voor elk product met een aantal groter dan 0
                foreach (var pa in vm.Producten)
                {
                    if (pa.Aantal > 0)
                    {
                        order.OrderRegels.Add(new OrderRegel
                        {
                            ProductId = pa.ProductId,
                            Quantity = pa.Aantal
                        });
                    }
                }
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            // Als het model niet geldig is, herlaad de producten en dropdown lijsten
            var producten = _context.Products.ToList();
            vm.Producten = producten.Select(p => new ProductAantal
            {
                ProductId = p.Id,
                ProductNaam = p.Name,
                Aantal = vm.Producten.FirstOrDefault(x => x.ProductId == p.Id)?.Aantal ?? 0
            }).ToList();
            ViewBag.GebruikersList = new SelectList(_context.Users.ToList(), "Id", "UserName", vm.UserId);
            ViewBag.KlantenList = new SelectList(_context.Customers.ToList(), "Id", "Name", vm.CustomerId);
            return View(vm);
        }

        // GET: Toont de bewerkingspagina voor een order (alleen voor gebruikers met permissie niveau 2)
        public async Task<IActionResult> Bewerken(int? id)
        {
            if (!UserHasPermission(2))
            {
                TempData["ErrorMessage"] = "You do not have permission to access that page.";
                return RedirectToAction("Index", "Home");
            }
            int userId = HttpContext.Session.GetObjectFromJson<int>("User_id");
            if (userId == 0)
                return Redirect("/User/Login");
            if (id == null)
                return NotFound();
            
            // Haal de order op met orderregels
            var order = _context.Orders
                .Include(o => o.OrderRegels)
                .FirstOrDefault(o => o.Id == id);
            if (order == null)
                return NotFound();
            
            // Vul het viewmodel met bestaande ordergegevens
            var producten = _context.Products.ToList();
            var vm = new KE03_INTDEV_SE_2_Base.Models.OrderAanmakenViewModel
            {
                OrderDate = order.OrderDate,
                CustomerId = order.CustomerId,
                UserId = order.UserId,
                Producten = producten.Select(p => new KE03_INTDEV_SE_2_Base.Models.ProductAantal
                {
                    ProductId = p.Id,
                    ProductNaam = p.Name,
                    Aantal = order.OrderRegels.FirstOrDefault(r => r.ProductId == p.Id)?.Quantity ?? 0
                }).ToList()
            };
            
            // Vul de dropdown lijsten met geselecteerde waarden
            ViewBag.GebruikersList = new SelectList(_context.Users.ToList(), "Id", "UserName", order.UserId);
            ViewBag.KlantenList = new SelectList(_context.Customers.ToList(), "Id", "Name", order.CustomerId);
            ViewBag.OrderId = order.Id;
            return View(vm);
        }

        // POST: Verwerkt het bewerken van een order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Bewerken(int id, KE03_INTDEV_SE_2_Base.Models.OrderAanmakenViewModel vm)
        {
            if (!UserHasPermission(2))
            {
                TempData["ErrorMessage"] = "You do not have permission to access that page.";
                return RedirectToAction("Index", "Home");
            }
            int userId = HttpContext.Session.GetObjectFromJson<int>("User_id");
            if (userId == 0)
                return Redirect("/User/Login");
            if (ModelState.IsValid)
            {
                // Haal de bestaande order op met orderregels
                var order = _context.Orders
                    .Include(o => o.OrderRegels)
                    .FirstOrDefault(o => o.Id == id);
                if (order == null)
                    return NotFound();
                
                // Update de ordergegevens
                order.OrderDate = vm.OrderDate;
                order.CustomerId = vm.CustomerId;
                order.UserId = vm.UserId;
                
                // Verwijder bestaande orderregels
                _context.RemoveRange(order.OrderRegels);
                
                // Voeg nieuwe orderregels toe
                foreach (var pa in vm.Producten)
                {
                    if (pa.Aantal > 0)
                    {
                        order.OrderRegels.Add(new DataAccessLayer.Models.OrderRegel
                        {
                            ProductId = pa.ProductId,
                            Quantity = pa.Aantal
                        });
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            // Als het model niet geldig is, herlaad de gegevens
            var producten = _context.Products.ToList();
            vm.Producten = producten.Select(p => new KE03_INTDEV_SE_2_Base.Models.ProductAantal
            {
                ProductId = p.Id,
                ProductNaam = p.Name,
                Aantal = vm.Producten.FirstOrDefault(x => x.ProductId == p.Id)?.Aantal ?? 0
            }).ToList();
            ViewBag.GebruikersList = new SelectList(_context.Users.ToList(), "Id", "UserName", vm.UserId);
            ViewBag.KlantenList = new SelectList(_context.Customers.ToList(), "Id", "Name", vm.CustomerId);
            ViewBag.OrderId = id;
            return View(vm);
        }

        // GET: Toont de bevestigingspagina voor het verwijderen van een order (alleen voor gebruikers met permissie niveau 2)
        public async Task<IActionResult> Verwijderen(int? id)
        {
            if (!UserHasPermission(2))
            {
                TempData["ErrorMessage"] = "You do not have permission to access that page.";
                return RedirectToAction("Index", "Home");
            }
            int userId = HttpContext.Session.GetObjectFromJson<int>("User_id");
            if (userId == 0)
                return Redirect("/User/Login");
            if (id == null)
                return NotFound();
            
            // Haal de order op met klant en gebruiker informatie
            var order = _context.Orders.Include(o => o.Customer).Include(o => o.User).FirstOrDefault(o => o.Id == id);
            if (order == null)
                return NotFound();
            return View(order);
        }

        // POST: Verwerkt het verwijderen van een order
        [HttpPost, ActionName("Verwijderen")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerwijderenBevestigd(int id)
        {
            if (!UserHasPermission(2))
            {
                TempData["ErrorMessage"] = "You do not have permission to access that page.";
                return RedirectToAction("Index", "Home");
            }
            int userId = HttpContext.Session.GetObjectFromJson<int>("User_id");
            if (userId == 0)
                return Redirect("/User/Login");
            
            // Zoek en verwijder de order
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
} 