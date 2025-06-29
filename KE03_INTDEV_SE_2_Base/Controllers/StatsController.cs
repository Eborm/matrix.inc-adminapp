using DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace KE03_INTDEV_SE_2_Base.Controllers;

// API Controller voor het ophalen van statistieken en rapportages
[Route("api/[controller]")]
[ApiController]
public class StatsController : ControllerBase
{
    // Database context voor directe database toegang
    private readonly MatrixIncDbContext _context;

    // Constructor voor dependency injection van database context
    public StatsController(MatrixIncDbContext context)
    {
        _context = context;
    }

    // GET: Haalt statistieken op van orders per maand
    // Retourneert een lijst met jaar, maand en aantal orders
    [HttpGet("orders-per-month")]
    public IActionResult GetOrdersPerMonth()
    {
        // Groepeer orders op jaar en maand en tel het aantal per groep
        var data = _context.Orders
            .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
            .Select(g => new { Year = g.Key.Year, Month = g.Key.Month, Count = g.Count() })
            .OrderBy(g => g.Year).ThenBy(g => g.Month)
            .ToList();

        return Ok(data);
    }

    // GET: Haalt alle actieve klanten op
    // Retourneert een lijst van klanten waarvan de Active property true is
    [HttpGet("active-users")]
    public IActionResult GetActiveUsers()
    {
        var data = _context.Customers.Where(c => c.Active).ToList();
        
        return Ok(data);
    }
    
    // GET: Controleert kortingen en toont top 10 producten op basis van order aantal
    // Retourneert product informatie inclusief order aantal en korting status
    [HttpGet("check-discount")]
    public async Task<ActionResult<IEnumerable<object>>> CheckDiscount()
    {
        try
        {
            // Haal producten op met order aantal en korting informatie
            var data = await _context.Products
                .Select(p => new 
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    OrderCount = p.Orders.Count,
                    Price = p.Price,
                    HasDiscount = p.Discount > 0
                })
                .OrderByDescending(x => x.OrderCount)
                .Take(10)
                .ToListAsync();

            if (!data.Any())
            {
                return NotFound("Geen producten gevonden");
            }

            return Ok(new
            {
                Message = "Er zijn nog geen orders geregistreerd voor deze producten",
                Products = data
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Er is een fout opgetreden bij het ophalen van de top producten");
        }
    }
}