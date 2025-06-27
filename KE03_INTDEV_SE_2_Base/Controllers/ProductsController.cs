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

public class ProductsController : Controller
{
    private readonly MatrixIncDbContext _context;
    private readonly ILogger<ProductsController> _logger;
    private readonly IProductRepository _productRepository;
    private readonly ILogsRepository _logsRepository;
    private readonly IUserRepository _UserRepository;

    private bool UserHasPermission(int requiredPermission)
    {
        int userId = HttpContext.Session.GetObjectFromJson<int>("User_id");
        if (userId == 0) return false;
        var user = _UserRepository.GetUserByUID(userId);
        return user != null && user.Permissions <= requiredPermission;
    }
    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.Id == id);
    }
    public ProductsController(MatrixIncDbContext context, ILogger<ProductsController> logger, IProductRepository productRepository, ILogsRepository logsRepository, IUserRepository UserRepository)
    {
        _logger = logger;
        _productRepository = productRepository;
        _context = context;
        _logsRepository = logsRepository;
        _UserRepository = UserRepository;
    }

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


    public IActionResult Create()
    {
        if (!UserHasPermission(1))
        {
            TempData["ErrorMessage"] = "You do not have permission to access that page.";
            return RedirectToAction("Index", "Home");
        }
        return View();
    }
    
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
                if (product.Discount != 0)
                {
                    _productRepository.SetDiscount(product.Id, product.Discount, product.DiscountStartDate, product.DiscountEndDate);
                }
                
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