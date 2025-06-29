using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;

namespace KE03_INTDEV_SE_2_Base.Controllers;

// Controller voor het beheren van producten in het systeem
public class ProductsController : Controller
{
    // Database context voor directe database toegang
    private readonly MatrixIncDbContext _context;
    
    // Logger voor het bijhouden van product-gerelateerde activiteiten
    private readonly ILogger<ProductsController> _logger;
    
    // Repository voor product-gerelateerde database operaties
    private readonly IProductRepository _productRepository;
    
    // Repository voor het bijhouden van logs van productacties
    private readonly ILogsRepository _logsRepository;
    
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
    
    // Controleert of een product bestaat in de database
    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.Id == id);
    }
    
    // Constructor voor dependency injection van alle benodigde services
    public ProductsController(MatrixIncDbContext context, ILogger<ProductsController> logger, IProductRepository productRepository, ILogsRepository logsRepository, IUserRepository UserRepository)
    {
        _logger = logger;
        _productRepository = productRepository;
        _context = context;
        _logsRepository = logsRepository;
        _UserRepository = UserRepository;
    }

    // Toont een overzicht van alle producten (alleen voor gebruikers met permissie niveau 2 of lager)
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
            return View(_productRepository.GetAllProducts());
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

    // Toont gedetailleerde informatie over een specifiek product (alleen voor managers en admins)
    public async Task<IActionResult> Details(int? id)
    {
        if (!UserHasPermission(1))
        {
            TempData["ErrorMessage"] = "You do not have permission to access that page.";
            return RedirectToAction("Index", "Home");
        }
        if (id == null)
        {
            return NotFound();
        }

        var product = _productRepository.GetProductById(id.Value);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    // Toont de pagina voor het aanmaken van een nieuw product (alleen voor managers en admins)
    public IActionResult Create()
    {
        if (!UserHasPermission(1))
        {
            TempData["ErrorMessage"] = "You do not have permission to access that page.";
            return RedirectToAction("Index", "Home");
        }
        return View();
    }
    
    // POST: Verwerkt het aanmaken van een nieuw product
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        if (!UserHasPermission(1))
        {
            TempData["ErrorMessage"] = "You do not have permission to access that page.";
            return RedirectToAction("Index", "Home");
        }
        _productRepository.AddProduct(product);
        
        // Log de actie van het aanmaken van een product
        int UID = HttpContext.Session.GetObjectFromJson<int>("User_id");
        string username = _UserRepository.GetUserByUID(UID).UserName;
        
        var log = new Log()
        {
            Action = "Add Product",
            User = username,
            Time = DateTime.Now,
            City = "could not be found"
        };
        
        await _logsRepository.AddLog(log);
        
        return RedirectToAction(nameof(Index));
    }
    
    // Toont de bewerkingspagina voor een product (alleen voor managers en admins)
    public async Task<IActionResult> Edit(int? id)
    {
        if (!UserHasPermission(1))
        {
            TempData["ErrorMessage"] = "You do not have permission to access that page.";
            return RedirectToAction("Index", "Home");
        }
        if (id == null)
        {
            return NotFound();
        }

        var product = _productRepository.GetProductById(id.Value);
        if (product == null)
        {
            return NotFound();
        }
        
        return View(product);
    }

    // POST: Verwerkt het bewerken van een product inclusief korting instellingen
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (!UserHasPermission(1))
        {
            TempData["ErrorMessage"] = "You do not have permission to access that page.";
            return RedirectToAction("Index", "Home");
        }
        if (id != product.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _productRepository.UpdateProduct(product);
                
                // Als er een korting is ingesteld, pas deze toe met start- en einddatum
                if (product.Discount != 0)
                {
                    _productRepository.SetDiscount(product.Id, product.Discount, product.DiscountStartDate, product.DiscountEndDate);
                }
                
                // Log de bewerking van het product
                int UID = HttpContext.Session.GetObjectFromJson<int>("User_id");
                string username = _UserRepository.GetUserByUID(UID).UserName;
        
                var log = new Log()
                {
                    Action = "Edit Product",
                    User = username,
                    Time = DateTime.Now,
                    City = "could not be found"
                };
        
                await _logsRepository.AddLog(log);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.Id))
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
        return View(product);
    }

    // Toont de bevestigingspagina voor het verwijderen van een product
    public async Task<IActionResult> Delete(int? id)
    {
        if (!UserHasPermission(1))
        {
            TempData["ErrorMessage"] = "You do not have permission to access that page.";
            return RedirectToAction("Index", "Home");
        }
        if (id == null)
        {
            return NotFound();
        }

        var customer = _productRepository.GetProductById(id.Value);
        if (customer == null)
        {
            return NotFound();
        }

        return View(customer);
    }
    
    // POST: Verwerkt het verwijderen van een product
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!UserHasPermission(1))
        {
            TempData["ErrorMessage"] = "You do not have permission to access that page.";
            return RedirectToAction("Index", "Home");
        }
        var product = _productRepository.GetProductById(id);

        if (product != null)
        {
            // Log de verwijdering van het product
            int UID = HttpContext.Session.GetObjectFromJson<int>("User_id");
            string username = _UserRepository.GetUserByUID(UID).UserName;
        
            var log = new Log()
            {
                Action = "Delete Product",
                User = username,
                Time = DateTime.Now,
                City = "could not be found"
            };
        
            await _logsRepository.AddLog(log);
            
            _productRepository.DeleteProduct(product);
        }
        return RedirectToAction(nameof(Index));
    }

}   