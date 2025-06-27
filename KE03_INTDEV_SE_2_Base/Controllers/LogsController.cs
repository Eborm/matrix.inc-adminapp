using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KE03_INTDEV_SE_2_Base.Controllers;

public class LogsController : Controller
{
    private readonly ILogger<LogsController> _logger;
    private readonly ILogsRepository _logsRepository;
    private readonly IUserRepository _UserRepository;

    private bool UserHasPermission(int requiredPermission)
    {
        int userId = HttpContext.Session.GetObjectFromJson<int>("User_id");
        if (userId == 0) return false;
        var user = _UserRepository.GetUserByUID(userId);
        return user != null && user.Permissions <= requiredPermission;
    }
    public LogsController(ILogsRepository logsRepository, ILogger<LogsController> logger, IUserRepository UserRepository)
    {
        _logsRepository = logsRepository;
        _logger = logger;
        _UserRepository = UserRepository;
    }

    public IActionResult Index()
    {
        if (!UserHasPermission(0))
        {
            TempData["ErrorMessage"] = "You do not have permission to access that page.";
            return RedirectToAction("Index", "Home");
        }
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