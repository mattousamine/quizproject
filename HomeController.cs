using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using quiz.Models;
using System.Diagnostics;

namespace quiz.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Retrieve session variables
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var userRole = HttpContext.Session.GetString("UserRole");
            bool isLoggedIn = userEmail != null;
            bool isAdmin = userRole == "Admin";

            // Redirect based on role
            if (!isLoggedIn)
            {
                // If the user is not logged in, redirect to Login action
                return RedirectToAction("Login", "Home");
            }
            else if (isAdmin)
            {
                // If the user is an admin, redirect to AdminPanel action in AdminController
                return RedirectToAction("AdminPanel", "Admin");
            }
            else
            {
                // If the user is logged in and not an admin, go user home page action in quizzes controller
                return RedirectToAction("userHomePage", "Quizzes");
            }
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Password()
        {
            return View();
        }
        public IActionResult Registre()
        {
            return View();
        }

        public IActionResult ResetPassword(int userId)
        {
            ViewBag.UserId = userId;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}