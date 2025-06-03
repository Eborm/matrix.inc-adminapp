using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using KE03_INTDEV_SE_2_Base.Models;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KE03_INTDEV_SE_2_Base.Controllers;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserRepository _UserRepository;

        public UserController(IUserRepository userRepository, ILogger<UserController> logger)
        {
            _logger = logger;
            _UserRepository = userRepository;
        }
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
                return View(user);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        // GET: UserController/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: UserController/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(User model)
        {
                User user = _UserRepository.GetUserByUserName(model.UserName);
            if (user != null)
            {
                if (user.Password == model.Password) // In a real application, you should hash the password and compare hashes
                {
                    HttpContext.Session.SetObjectAsJson("User_id", user.Id);
                    _logger.LogInformation($"User {user.UserName} logged in successfully.");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                    _logger.LogWarning($"Failed login attempt for user {model.UserName}.");
                    return View();
                }
            }
            return View();

        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User model)
        {
            User user = new User
            {
                UserName = model.UserName,
                Password = "password", //Stock password
                Permissions = model.Permissions 
            };
            _UserRepository.AddUser(user);
            return RedirectToAction("Index", "Home");
        }


        // Stub methods — add logic as needed
        public IActionResult Edit(int id) => View();
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(int id, User model) => RedirectToAction(nameof(Index));

        public IActionResult Delete()
        {
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Delete(User model)
        {
            User user = _UserRepository.GetUserByUserName(model.UserName);
            if (user != null)
            {
                _UserRepository.DeleteUser(user);
                _logger.LogInformation($"User {user.UserName} deleted successfully.");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return NotFound();
            }
        }
    }
}
