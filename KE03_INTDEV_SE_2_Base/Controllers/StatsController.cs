using DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
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

}
