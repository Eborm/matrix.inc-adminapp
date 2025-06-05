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
                _logger.LogInformation(_UserRepository.EncryptPassword(model.Password));
                if (user.Password == _UserRepository.EncryptPassword(model.Password))
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
                Password = _UserRepository.EncryptPassword("password"), //Stock password
                Permissions = model.Permissions
            };
            _UserRepository.AddUser(user);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Delete(int id)
        {
            User user = _UserRepository.GetUserByUID(id);
            return View(user);
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

        public IActionResult settings()
        {
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Settings(Userupdate userupdate)
        {
            int Id = HttpContext.Session.GetObjectFromJson<int>("User_id");
            if (Id != 0)
            {
                User user = _UserRepository.GetUserByUID(Id);
                if (user != null)
                {
                    if (user.Password == _UserRepository.EncryptPassword(userupdate.Password))
                    {
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
                return Redirect("/Home/Index");
            }
            else
            {
                return Redirect("/User/Login");
            }
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _UserRepository.GetUserByUID(id.Value);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            User new_user = _UserRepository.GetUserByUID(id);
            new_user.UserName = user.UserName;
            new_user.Permissions = user.Permissions;
            if (id != user.Id)
            {
                return NotFound();
            }
            try
            {
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
