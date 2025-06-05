using DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace KE03_INTDEV_SE_2_Base.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StatsController : ControllerBase
{
    private readonly MatrixIncDbContext _context;

    public StatsController(MatrixIncDbContext context)
    {
        _context = context;
    }

    [HttpGet("orders-per-month")]
    public IActionResult GetOrdersPerMonth()
    {
        var data = _context.Orders
            .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
            .Select(g => new { Year = g.Key.Year, Month = g.Key.Month, Count = g.Count() })
            .OrderBy(g => g.Year).ThenBy(g => g.Month)
            .ToList();

        return Ok(data);
    }

    [HttpGet("active-users")]
    public IActionResult GetActiveUsers()
    {
        var data = _context.Customers.Where(c => c.Active).ToList();
        
        return Ok(data);
    }
    
    [HttpGet("check-discount")]
    public async Task<ActionResult<IEnumerable<object>>> CheckDiscount()
    {
        try
        {
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