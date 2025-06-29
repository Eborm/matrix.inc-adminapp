using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using KE03_INTDEV_SE_2_Base.Controllers;
using KE03_INTDEV_SE_2_Base.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    // Controller voor het beheren van gebruikers, authenticatie en autorisatie
    public class UserController : Controller
    {
        // Controleert of de huidige gebruiker de vereiste permissies heeft
        // requiredPermission: 0 = admin, hogere getallen = minder rechten
        private bool UserHasPermission(int requiredPermission)
        {
            int userId = HttpContext.Session.GetObjectFromJson<int>("User_id");
            if (userId == 0) return false;
            var user = _UserRepository.GetUserByUID(userId);
            return user != null && user.Permissions <= requiredPermission;
        }
        
        // Logger voor het bijhouden van gebruikersactiviteiten en fouten
        private readonly ILogger<UserController> _logger;
        
        // Repository voor gebruikers-gerelateerde database operaties
        private readonly IUserRepository _UserRepository;
        
        // Repository voor het bijhouden van logs van gebruikersacties
        private readonly ILogsRepository _LogsRepository;

        // Constructor voor dependency injection van repositories en logger
        public UserController(IUserRepository userRepository, ILogger<UserController> logger, ILogsRepository LogsRepository)
        {
            _logger = logger;
            _UserRepository = userRepository;
            _LogsRepository = LogsRepository;
        }
        
        // Hoofdpagina voor gebruikers - controleert sessie en stuurt door naar juiste pagina
        public IActionResult Index()
        {
            int Id = HttpContext.Session.GetObjectFromJson<int>("User_id");
            User user = null;
            if (Id != 0)
            {
                user = _UserRepository.GetUserByUID(Id);
            }
            if (user != null)
            {
                _logger.LogInformation($"User {user.UserName} accessed the index page.");
                return RedirectToAction("settings");
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

        // GET: Toont de login pagina
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Verwerkt de login gegevens en controleert authenticatie
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User model)
        {
            User user = _UserRepository.GetUserByUserName(model.UserName);
            if (user != null)
            {
                _logger.LogInformation(_UserRepository.EncryptPassword(model.Password));
                if (user.Password == _UserRepository.EncryptPassword(model.Password))
                {
                    // Sla gebruikers-ID op in sessie voor authenticatie
                    HttpContext.Session.SetObjectAsJson("User_id", user.Id);
                    _logger.LogInformation($"User {user.UserName} logged in successfully.");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                    _logger.LogWarning($"Failed login attempt for user {model.UserName}.");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password.");
                _logger.LogWarning($"Failed login attempt for user {model.UserName}.");
                return View(model);
            }

        }

        // Toont de pagina voor het aanmaken van een nieuwe gebruiker (alleen voor admins)
        public IActionResult Create()
        {
            if (!UserHasPermission(0)) // 0 = admin, for example
            {
                    TempData["ErrorMessage"] = "You do not have permission to access that page.";
                    return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Verwerkt het aanmaken van een nieuwe gebruiker
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User model)
        {
            string username = model.UserName;
        
            // Log de actie van het aanmaken van een gebruiker
            var log = new Log()
            {
                Action = "Add User",
                User = username,
                Time = DateTime.Now,
                City = "could not be found"
            };
        
            await _LogsRepository.AddLog(log);
            
            // Maak nieuwe gebruiker aan met standaard wachtwoord
            User user = new User
            {
                UserName = model.UserName,
                Password = _UserRepository.EncryptPassword("password"), //Stock password
                Permissions = model.Permissions
            };
            _UserRepository.AddUser(user);
            return RedirectToAction("Index", "Home");
        }

        // Toont de bevestigingspagina voor het verwijderen van een gebruiker
        public IActionResult Delete(int id)
        {
            if (!UserHasPermission(0))
            {
                    TempData["ErrorMessage"] = "You do not have permission to access that page.";
                    return RedirectToAction("Index", "Home");
            }
            User user = _UserRepository.GetUserByUID(id);
            return View(user);
        }
        
        // POST: Verwerkt het verwijderen van een gebruiker
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(User model)
        {
            if (!UserHasPermission(0))
            {
                    TempData["ErrorMessage"] = "You do not have permission to access that page.";
                    return RedirectToAction("Index", "Home");
            }
            User user = _UserRepository.GetUserByUserName(model.UserName);
            if (user != null)
            {
                int UID = HttpContext.Session.GetObjectFromJson<int>("User_id");
                string username = _UserRepository.GetUserByUID(UID).UserName;
        
                // Log de verwijdering van de gebruiker
                var log = new Log()
                {
                    Action = "Delete User",
                    User = username,
                    Time = DateTime.Now,
                    City = "could not be found"
                };
        
                await _LogsRepository.AddLog(log);
                _UserRepository.DeleteUser(user);
                _logger.LogInformation($"User {user.UserName} deleted successfully.");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return NotFound();
            }
        }

        // Toont de instellingenpagina voor de huidige gebruiker
        public IActionResult settings()
        {
            return View();
        }
        
        // POST: Verwerkt het wijzigen van gebruikersinstellingen (wachtwoord wijzigen)
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Settings(Userupdate userupdate)
        {
            int Id = HttpContext.Session.GetObjectFromJson<int>("User_id");
            if (Id != 0)
            {
                User user = _UserRepository.GetUserByUID(Id);
                if (user != null)
                {
                    // Controleer of het huidige wachtwoord correct is
                    if (user.Password == _UserRepository.EncryptPassword(userupdate.Password))
                    {
                        // Update het wachtwoord naar het nieuwe wachtwoord
                        user.Password = _UserRepository.EncryptPassword(userupdate.Password_new);
                        _UserRepository.UpdateUser(user);
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    _logger.LogWarning($"User with ID {Id} not found for settings.");
                    return RedirectToAction("Login");
                }
            }
            else
            {
                return View();
            }
        }
        
        // Toont het admin panel met alle gebruikers (alleen voor admins)
        public IActionResult panel()
        {
            int Id = HttpContext.Session.GetObjectFromJson<int>("User_id");
            User user = null;
            if (Id != 0)
            {
                user = _UserRepository.GetUserByUID(Id);
            }
            if (user != null && user.Permissions == 0)
            {
                _logger.LogInformation($"User {user.UserName} accessed the index page.");
                return View(_UserRepository.GetAllUsers());
            }
            else if (user != null)
            {
                TempData["ErrorMessage"] = "You do not have permission to access that page.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["ErrorMessage"] = "You do not have permission to access that page.";
                return RedirectToAction("Login", "User");
            }
        }
        
        // Toont de bewerkingspagina voor een gebruiker (alleen voor admins)
        public async Task<IActionResult> Edit(int? id)
        {
            var user = _UserRepository.GetUserByUID(id.Value);
            if (!UserHasPermission(0))
            {
                    TempData["ErrorMessage"] = "You do not have permission to access that page.";
                    return RedirectToAction("Index", "Home");
            }
            else if (id == null)
            {
                return NotFound();
            }

            else if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Verwerkt het bewerken van een gebruiker
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (!UserHasPermission(0))
            {
                    TempData["ErrorMessage"] = "You do not have permission to access that page.";
                    return RedirectToAction("Index", "Home");
            }
            
            // Haal de bestaande gebruiker op en update de gegevens
            User new_user = _UserRepository.GetUserByUID(id);
            new_user.UserName = user.UserName;
            new_user.Permissions = user.Permissions;
            if (id != user.Id)
            {
                return NotFound();
            }
            try
            {
                int UID = HttpContext.Session.GetObjectFromJson<int>("User_id");
                string username = _UserRepository.GetUserByUID(UID).UserName;
        
                // Log de bewerking van de gebruiker
                var log = new Log()
                {
                    Action = "Edit User",
                    User = username,
                    Time = DateTime.Now,
                    City = "could not be found"
                };
        
                await _LogsRepository.AddLog(log);
                _UserRepository.UpdateUser(new_user);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_UserRepository.GetUserByUID(user.Id) == null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("panel");
        }

    }
}
