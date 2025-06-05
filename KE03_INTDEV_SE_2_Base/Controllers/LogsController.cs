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

    public LogsController(ILogsRepository logsRepository, ILogger<LogsController> logger, IUserRepository UserRepository)
    {
        _logsRepository = logsRepository;
        _logger = logger;
        _UserRepository = UserRepository;
    }

    public IActionResult Index()
    {
        int Id = HttpContext.Session.GetObjectFromJson<int>("User_id");
        User user = null;
        if (Id != 0)
        {
            user = _UserRepository.GetUserByUID(Id);
        }
        if (user != null && user.Permissions <= 2)
        {
            return View(_logsRepository.GetAllLogs());
        }
        else
        {
            return RedirectToPage("/User/Login");
        }
    }
}