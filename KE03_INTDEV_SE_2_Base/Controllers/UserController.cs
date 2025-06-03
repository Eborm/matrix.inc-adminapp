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

            return RedirectToAction("Login");
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
                user.Password = model.Password; // This should be hashed in a real application
                HttpContext.Session.SetObjectAsJson("User_id", user.Id);
                _logger.LogInformation($"User {user.UserName} logged in successfully.");
                return RedirectToAction("Index", "Home");
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
            if (ModelState.IsValid)
            {
                // Handle creation logic
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // Stub methods — add logic as needed
        public IActionResult Edit(int id) => View();
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(int id, User model) => RedirectToAction(nameof(Index));

        public IActionResult Delete(int id) => View();
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Delete(int id, IFormCollection collection) => RedirectToAction(nameof(Index));
    }
}
