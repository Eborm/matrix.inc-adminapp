using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KE03_INTDEV_SE_2_Base.Controllers;

public class OrdersController : Controller
{
    private readonly MatrixIncDbContext _context;
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;

    public OrdersController(MatrixIncDbContext context, IOrderRepository orderRepository, ICustomerRepository customerRepository, IProductRepository productRepository)
    {
        _context = context;
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
    }

    public IActionResult Index()
    {
        var orders = _orderRepository.GetAllOrders();
        return View(orders);
    }

    public IActionResult Create()
    {
        ViewBag.Customers = new SelectList(_customerRepository.GetAllCustomers(), "Id", "Name");
        ViewBag.Products = new MultiSelectList(_productRepository.GetAllProducts(), "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Order order, int[] selectedProducts)
    {
        try
        {
            if (ModelState.IsValid)
            {
                foreach (var productId in selectedProducts)
                {
                    var product = _productRepository.GetProductById(productId);
                    if (product != null)
                    {
                        _context.Attach(product);
                        order.Products.Add(product);
                    }
                }
                order.OrderDate = DateTime.Now;
                _orderRepository.AddOrder(order);
                TempData["Success"] = "Order succesvol opgeslagen.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Error"] = "ModelState is niet geldig.";
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Fout bij opslaan order: {ex.Message}";
        }
        ViewBag.Customers = new SelectList(_customerRepository.GetAllCustomers(), "Id", "Name", order.CustomerId);
        ViewBag.Products = new MultiSelectList(_productRepository.GetAllProducts(), "Id", "Name", selectedProducts);
        return View(order);
    }
} 