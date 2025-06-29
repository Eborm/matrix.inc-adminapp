using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    // Controller voor het beheren van klanten in het systeem
    public class CustomersController : Controller
    {
        // Database context voor directe database toegang
        private readonly MatrixIncDbContext _context;
        
        // Repository voor gebruikers-gerelateerde operaties
        private readonly IUserRepository _UserRepository;
        
        // Repository voor het bijhouden van logs van klantacties
        private readonly ILogsRepository _logsRepository;
        
        // Logger voor het bijhouden van klant-gerelateerde activiteiten
        private readonly ILogger<CustomersController> _logger;
        
        // Controleert of een klant bestaat in de database
        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }

        // Controleert of de huidige gebruiker de vereiste permissies heeft
        // requiredPermission: 0 = admin, 1 = manager, 2 = gebruiker
        private bool UserHasPermission(int requiredPermission)
        {
            int userId = HttpContext.Session.GetObjectFromJson<int>("User_id");
            if (userId == 0) return false;
            var user = _UserRepository.GetUserByUID(userId);
            return user != null && user.Permissions <= requiredPermission;
        }
        
        // Constructor voor dependency injection van alle benodigde services
        public CustomersController(IUserRepository userRepository, MatrixIncDbContext context, ILogger<CustomersController> logger, IUserRepository UserRepository, ILogsRepository LogsRepository)
        {
            _context = context;
            _UserRepository = UserRepository;
            _logsRepository = LogsRepository;
            _logger = logger;
            _UserRepository = userRepository;
        }

        // GET: Toont een overzicht van alle klanten (alleen voor gebruikers met permissie niveau 2 of lager)
        public async Task<IActionResult> Index()
        {
            int Id = HttpContext.Session.GetObjectFromJson<int>("User_id");
            User user = null;
            if (Id != 0)
            {
                user = _UserRepository.GetUserByUID(Id);
            }
            if (user != null && user.Permissions <= 2)
            {
                return View(await _context.Customers.ToListAsync());
            }
            else if (user != null)
            {
                return Redirect("/Home/Index");
            }
            else
            {
                return Redirect("/User/Login");
            }
        }

        // GET: Toont gedetailleerde informatie over een specifieke klant (alleen voor gebruikers met permissie niveau 2)
        public async Task<IActionResult> Details(int? id)
        {
            if (!UserHasPermission(2))
            {
                TempData["ErrorMessage"] = "You do not have permission to access that page.";
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Toont de pagina voor het aanmaken van een nieuwe klant
        public IActionResult Create()
        {
            return View();
        }

        // POST: Verwerkt het aanmaken van een nieuwe klant
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,Active")] Customer customer)
        {
            if (!UserHasPermission(2))
            {
                TempData["ErrorMessage"] = "You do not have permission to access that page.";
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                
                // Log de actie van het aanmaken van een klant
                int UID = HttpContext.Session.GetObjectFromJson<int>("User_id");
                string username = _UserRepository.GetUserByUID(UID).UserName;
        
                var log = new Log()
                {
                    Action = "Add Customer",
                    User = username,
                    Time = DateTime.Now,
                    City = "could not be found"
                };
        
                await _logsRepository.AddLog(log);
                
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Toont de bewerkingspagina voor een klant (alleen voor gebruikers met permissie niveau 2)
        public async Task<IActionResult> Edit(int? id)
        {
            if (!UserHasPermission(2))
            {
                TempData["ErrorMessage"] = "You do not have permission to access that page.";
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Verwerkt het bewerken van een klant
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Active")] Customer customer)
        {
            if (!UserHasPermission(2))
            {
                TempData["ErrorMessage"] = "You do not have permission to access that page.";
                return RedirectToAction("Index", "Home");
            }
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                    
                    // Log de bewerking van de klant
                    int UID = HttpContext.Session.GetObjectFromJson<int>("User_id");
                    string username = _UserRepository.GetUserByUID(UID).UserName;
        
                    var log = new Log()
                    {
                        Action = "Edit Customer",
                        User = username,
                        Time = DateTime.Now,
                        City = "could not be found"
                    };
        
                    await _logsRepository.AddLog(log);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Toont de bevestigingspagina voor het verwijderen van een klant (alleen voor gebruikers met permissie niveau 2)
        public async Task<IActionResult> Delete(int? id)
        {
            if (!UserHasPermission(2))
            {
                TempData["ErrorMessage"] = "You do not have permission to access that page.";
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Verwerkt het verwijderen van een klant
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!UserHasPermission(2))
            {
                TempData["ErrorMessage"] = "You do not have permission to access that page.";
                return RedirectToAction("Index", "Home");
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                // Log de verwijdering van de klant
                int UID = HttpContext.Session.GetObjectFromJson<int>("User_id");
                string username = _UserRepository.GetUserByUID(UID).UserName;
        
                var log = new Log()
                {
                    Action = "Delete Customer",
                    User = username,
                    Time = DateTime.Now,
                    City = "could not be found"
                };
        
                await _logsRepository.AddLog(log);
                
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
