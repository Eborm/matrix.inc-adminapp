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
    public class CustomersController : Controller
    {
        private readonly MatrixIncDbContext _context;
        private readonly IUserRepository _UserRepository;
        private readonly ILogsRepository _logsRepository;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(MatrixIncDbContext context, ILogger<CustomersController> logger, IUserRepository UserRepository, ILogsRepository LogsRepository)
        {
            _context = context;
            _UserRepository = UserRepository;
            _logsRepository = LogsRepository;
            _logger = logger;
        }

        // GET: Customers
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

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
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

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,Active")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                
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

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
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

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Active")] Customer customer)
        {
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

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
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

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
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

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
