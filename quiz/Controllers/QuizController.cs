using Microsoft.AspNetCore.Mvc;

namespace quiz.Controllers
{
    public class QuizController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Simple()
        {
            return View();
        }
        public IActionResult Multiple()
        {
            return View();
        }
    }
}
