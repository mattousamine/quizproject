using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quiz.DAL; 
using quiz.Models;
using System.Threading.Tasks;

namespace quiz.Controllers
{
    public class AdminController : Controller
    {
        private readonly QuizContext _context;

        public AdminController(QuizContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> AdminPanel()
        {
            var viewModel = new AdminPanelCount
            {
                Quizzes = await _context.UserQuizzes.ToListAsync(),
                CategoryCount = await _context.Categories.CountAsync(),
                QuizCount = await _context.Quizzes.CountAsync(),
                QuestionCount = await _context.Questions.CountAsync(),
                OptionCount = await _context.Options.CountAsync(),
            };

            return View(viewModel);
        }
        [HttpGet]
        public async Task<JsonResult> GetStats()
        {
            var stats = new
            {
                CategoryCount = await _context.Categories.CountAsync(),
                QuizCount = await _context.Quizzes.CountAsync(),
                QuestionCount = await _context.Questions.CountAsync(),
                OptionCount = await _context.Options.CountAsync()
            };

            return Json(stats);
        }

        [HttpGet]
        public async Task<JsonResult> GetBarChartStats()
        {
            var stats = new
            {
                Labels = new string[] { "Category", "Quiz", "Question", "Option" },
                Counts = new int[]
                {
            await _context.Categories.CountAsync(),
            await _context.Quizzes.CountAsync(),
            await _context.Questions.CountAsync(),
            await _context.Options.CountAsync()
                }
            };

            return Json(stats);
        }

    }
}
