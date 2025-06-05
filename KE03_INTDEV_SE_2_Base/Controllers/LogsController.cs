using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace KE03_INTDEV_SE_2_Base.Controllers;

public class LogsController : Controller
{
    private readonly ILogger<LogsController> _logger;
    private readonly ILogsRepository _logsRepository;

    public LogsController(ILogsRepository logsRepository, ILogger<LogsController> logger)
    {
        _logsRepository = logsRepository;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View(_logsRepository.GetAllLogs());
    }
}