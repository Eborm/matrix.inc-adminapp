using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KE03_INTDEV_SE_2_Base.Controllers;

// Controller voor het bekijken van systeem logs (alleen voor admins en managers)
public class LogsController : Controller
{
    // Logger voor het bijhouden van controller activiteiten
    private readonly ILogger<LogsController> _logger;
    
    // Repository voor het ophalen van logs uit de database
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
    
    // Constructor voor dependency injection van repositories en logger
    public LogsController(ILogsRepository logsRepository, ILogger<LogsController> logger, IUserRepository UserRepository)
    {
        _logsRepository = logsRepository;
        _logger = logger;
        _UserRepository = UserRepository;
    }

    // Toont een overzicht van alle systeem logs (alleen voor admins en managers)
    public IActionResult Index()
    {
        int Id = HttpContext.Session.GetObjectFromJson<int>("User_id");
        User user = null;
        if (Id != 0)
        {
            user = _UserRepository.GetUserByUID(Id);
        }
        if (user != null && user.Permissions <= 1)
        {
            return View(_logsRepository.GetAllLogs());
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
}