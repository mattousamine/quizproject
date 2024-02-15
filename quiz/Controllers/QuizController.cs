using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using quiz.DAL;
using quiz.Models;

namespace quiz.Controllers
{
    public class QuizController : Controller
    {
        private readonly QuizContext _context;
        public QuizController(QuizContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> GetQuestions(int quizId, short level)
        {
            if (level != 1)
            {
                return BadRequest("Currently, only level 1 questions are supported.");
            }

            var quizExists = await _context.Quizzes.AnyAsync(q => q.QuizId == quizId && q.QuizIsActive);
            if (!quizExists)
            {
                return NotFound($"Quiz with ID {quizId} not found or is not active.");
            }

            var questions = await _context.QuizQuestions
                .Where(qq => qq.QuizId == quizId && qq.Question.QuestionLevel == level && qq.Question.QuestionIsActive)
                .Select(qq => new { qq.Question.QuestionId, qq.Question.QuestionText })
                .Take(5)
                .ToListAsync();

            if (!questions.Any())
            {
                return NotFound("No questions found for this quiz and level.");
            }

            // Fetch all options for these questions in one go
            var questionIds = questions.Select(q => q.QuestionId).ToList();
            var allOptions = await _context.QuestionOptions
                .Where(qo => questionIds.Contains(qo.QuestionId))
                .Select(qo => new { qo.QuestionId, qo.Option.OptionText, qo.Option.OptionIsCorrect })
                .ToListAsync();

            // Organize options by question
            var questionsWithOptions = questions.Select(q => new
            {
                q.QuestionId,
                q.QuestionText,
                Options = allOptions.Where(o => o.QuestionId == q.QuestionId)
                                    .Select(o => new { o.OptionText, o.OptionIsCorrect })
                                    .ToList()
            }).ToList();

            // Prepare the final structure
            var data = new
            {
                questions = questionsWithOptions.Select(q => new
                {
                    Text = q.QuestionText,
                    Options = q.Options.Select(o => o.OptionText).ToList(),
                    CorrectAnswer = q.Options.Where(o => o.OptionIsCorrect).Select(o => o.OptionText).FirstOrDefault()
                }).ToList(),
                answers = questionsWithOptions
                            .SelectMany(q => q.Options.Where(o => o.OptionIsCorrect).Select(o => new { QuestionText = q.QuestionText, o.OptionText }))
                            .GroupBy(q => q.OptionText)
                            .ToDictionary(g => g.Key, g => g.Select(q => q.QuestionText).ToList())
            };

            return Json(data);
        }



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
