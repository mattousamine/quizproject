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

        public async Task<IActionResult> GetAllCategoriesWithDetails()
        {
            var categoriesWithDetails = await _context.Categories
                .Where(c => c.CategoryIsActive) // Filter active categories if necessary
                .Select(c => new
                {
                    c.CategoryId,
                    c.CategoryName,
                    Quizzes = _context.CategoryQuizzes
                        .Where(cq => cq.CategoryId == c.CategoryId && cq.Quiz.QuizIsActive) // Join with Quizzes through CategoryQuizzes
                        .Select(cq => new
                        {
                            cq.Quiz.QuizId,
                            cq.Quiz.QuizName,
                            Questions = _context.QuizQuestions
                                .Where(qq => qq.QuizId == cq.QuizId && qq.Question.QuestionIsActive) // Join with Questions through QuizQuestions
                                .Select(qq => new
                                {
                                    qq.Question.QuestionId,
                                    qq.Question.QuestionText,
                                    Options = _context.QuestionOptions
                                        .Where(qo => qo.QuestionId == qq.QuestionId) // Join with Options through QuestionOptions
                                        .Select(qo => new
                                        {
                                            qo.Option.OptionId,
                                            qo.Option.OptionText,
                                            qo.Option.OptionIsCorrect
                                        }).ToList() // Materialize the inner collection
                                }).ToList() // Materialize the nested collection
                        }).ToList() // Materialize the outer collection
                })
                .ToListAsync();

            return View("~/Views/Admin/GetAllCategoriesWithDetails.cshtml", categoriesWithDetails);
        }





    }
}
