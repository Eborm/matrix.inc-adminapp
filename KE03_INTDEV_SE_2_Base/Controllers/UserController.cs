using Microsoft.AspNetCore.Mvc;
using KE03_INTDEV_SE_2_Base.Models;
using DataAccessLayer.Models;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    public class UserController : Controller
    {
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
            if (ModelState.IsValid)
            {
                // TODO: Validate user credentials

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
