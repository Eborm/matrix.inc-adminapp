using DataAccessLayer;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using KE03_INTDEV_SE_2_Base.Models;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    public class OrdersController : Controller
    {
        private readonly MatrixIncDbContext _context;

        public OrdersController(MatrixIncDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            int userId = HttpContext.Session.GetObjectFromJson<int>("User_id");
            if (userId == 0)
                return Redirect("/User/Login");
            var orders = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.User)
                .Include(o => o.OrderRegels).ThenInclude(or => or.Product)
                .ToList();
            return View(orders);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            int userId = HttpContext.Session.GetObjectFromJson<int>("User_id");
            if (userId == 0)
                return Redirect("/User/Login");
            if (id == null)
                return NotFound();
            var order = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.User)
                .Include(o => o.OrderRegels).ThenInclude(or => or.Product)
                .FirstOrDefault(o => o.Id == id);
            if (order == null)
                return NotFound();
            return View(order);
        }

        // GET: Orders/Aanmaken
        public IActionResult Aanmaken()
        {
            int userId = HttpContext.Session.GetObjectFromJson<int>("User_id");
            if (userId == 0)
                return Redirect("/User/Login");
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
            ViewBag.GebruikersList = new SelectList(_context.Users.ToList(), "Id", "UserName");
            ViewBag.KlantenList = new SelectList(_context.Customers.ToList(), "Id", "Name");
            return View(vm);
        }

        // POST: Orders/Aanmaken
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Aanmaken(OrderAanmakenViewModel vm)
        {
            int userId = HttpContext.Session.GetObjectFromJson<int>("User_id");
            if (userId == 0)
                return Redirect("/User/Login");
            if (ModelState.IsValid)
            {
                var order = new Order
                {
                    OrderDate = vm.OrderDate,
                    CustomerId = vm.CustomerId,
                    UserId = vm.UserId
                };
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

        // GET: Orders/Bewerken/5
        public async Task<IActionResult> Bewerken(int? id)
        {
            int userId = HttpContext.Session.GetObjectFromJson<int>("User_id");
            if (userId == 0)
                return Redirect("/User/Login");
            if (id == null)
                return NotFound();
            var order = _context.Orders
                .Include(o => o.OrderRegels)
                .FirstOrDefault(o => o.Id == id);
            if (order == null)
                return NotFound();
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
            ViewBag.GebruikersList = new SelectList(_context.Users.ToList(), "Id", "UserName", order.UserId);
            ViewBag.KlantenList = new SelectList(_context.Customers.ToList(), "Id", "Name", order.CustomerId);
            ViewBag.OrderId = order.Id;
            return View(vm);
        }

        // POST: Orders/Bewerken/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Bewerken(int id, KE03_INTDEV_SE_2_Base.Models.OrderAanmakenViewModel vm)
        {
            int userId = HttpContext.Session.GetObjectFromJson<int>("User_id");
            if (userId == 0)
                return Redirect("/User/Login");
            if (ModelState.IsValid)
            {
                var order = _context.Orders
                    .Include(o => o.OrderRegels)
                    .FirstOrDefault(o => o.Id == id);
                if (order == null)
                    return NotFound();
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

        // GET: Orders/Verwijderen/5
        public async Task<IActionResult> Verwijderen(int? id)
        {
            int userId = HttpContext.Session.GetObjectFromJson<int>("User_id");
            if (userId == 0)
                return Redirect("/User/Login");
            if (id == null)
                return NotFound();
            var order = _context.Orders.Include(o => o.Customer).Include(o => o.User).FirstOrDefault(o => o.Id == id);
            if (order == null)
                return NotFound();
            return View(order);
        }

        // POST: Orders/Verwijderen/5
        [HttpPost, ActionName("Verwijderen")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerwijderenBevestigd(int id)
        {
            int userId = HttpContext.Session.GetObjectFromJson<int>("User_id");
            if (userId == 0)
                return Redirect("/User/Login");
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