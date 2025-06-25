using System;
using System.Collections.Generic;
using System.Linq;
using DataAccessLayer;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KE03_INTDEV_SE_2_Base.Controllers;

public class ProductsController : Controller
{
    private readonly MatrixIncDbContext _context;
    private readonly ILogger<ProductsController> _logger;
    private readonly IProductRepository _productRepository;

    public ProductsController(MatrixIncDbContext context, ILogger<ProductsController> logger, IProductRepository productRepository)
    {
        _logger = logger;
        _productRepository = productRepository;
        _context = context;
    }
    
    public IActionResult Index()
    {
        return View(_productRepository.GetAllProducts());
    }

    public IActionResult Details(int? id)
    {
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
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Product product)
    {
        product.Discount = product.Discount / 100m;
        product.DiscountDuration = product.DiscountDuration * 86400; // dagen naar seconden
        if (product.Discount > 0)
        {
            product.DiscountStartTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
        _productRepository.AddProduct(product);
        return RedirectToAction(nameof(Index));
    }
    
    public IActionResult Edit(int? id)
    {
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
    public IActionResult Edit(int id, Product product)
    {
        if (id != product.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            product.Discount = product.Discount / 100m;
            product.DiscountDuration = product.DiscountDuration * 86400; // dagen naar seconden
            if (product.Discount > 0)
            {
                product.DiscountStartTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
            _productRepository.UpdateProduct(product);
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    public IActionResult Delete(int? id)
    {
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

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var product = _productRepository.GetProductById(id);
        if (product != null)
        {
            _productRepository.DeleteProduct(product);
        }
        return RedirectToAction(nameof(Index));
    }

    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.Id == id);
    }
}   