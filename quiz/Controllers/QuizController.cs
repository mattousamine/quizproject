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
            if (level < 1 || level > 3)
            {
                return BadRequest("Currently, only levels 1 to 3 questions are supported.");
            }

            var quizExists = await _context.Quizzes.AnyAsync(q => q.QuizId == quizId && q.QuizIsActive);
            if (!quizExists)
            {
                return NotFound($"Quiz with ID {quizId} not found or is not active.");
            }

            var questions = await _context.QuizQuestions
                .Where(qq => qq.QuizId == quizId && qq.Question.QuestionLevel == level && qq.Question.QuestionIsActive)
                .Select(qq => new
                {
                    qq.Question.QuestionId,
                    qq.Question.QuestionText,
                })
                .ToListAsync();

            if (!questions.Any())
            {
                return NotFound($"No questions found for this quiz and level {level}.");
            }

            // Fetch all options for these questions in one go
            var questionIds = questions.Select(q => q.QuestionId).ToList();
            var allOptions = await _context.QuestionOptions
                .Where(qo => questionIds.Contains(qo.QuestionId))
                .Select(qo => new { qo.QuestionId, qo.Option.OptionText, qo.Option.OptionIsCorrect })
                .ToListAsync();

            if (level == 1)
            {
                // Organize options by question
                var questionsWithOptions = questions.Select(q => new
                {
                    q.QuestionId,
                    // Utilisez des balises HTML pour formater le texte, par exemple <strong> pour le gras
                    QuestionText = FormatTextForDisplay(q.QuestionText),
                    Options = allOptions.Where(o => o.QuestionId == q.QuestionId)
                            .Select(o => new { OptionText = FormatTextForDisplay(o.OptionText), o.OptionIsCorrect })
                            .ToList()
                }).ToList();

                // Prepare the final structure
                var data = new
                {
                    questions = questionsWithOptions.Select(q => new
                    {
                        Text = EncodeForJavaScript(q.QuestionText),
                        Options = q.Options.Select(o => EncodeForJavaScript(o.OptionText)).ToList(),
                        CorrectAnswer = q.Options.Where(o => o.OptionIsCorrect).Select(o => EncodeForJavaScript(o.OptionText)).FirstOrDefault()
                    }).ToList(),
                    answers = questionsWithOptions
                    .SelectMany(q => q.Options.Where(o => o.OptionIsCorrect).Select(o => new { QuestionText = EncodeForJavaScript(q.QuestionText), OptionText = EncodeForJavaScript(o.OptionText) }))
                    .GroupBy(q => q.OptionText)
                    .ToDictionary(g => g.Key, g => g.Select(q => q.QuestionText).ToList())
                };

                return Json(data);
            }
            else
            {
                // Levels 2 and 3 format
                var difficultyKey = "easy";
                var quizData = new Dictionary<string, List<object>>
                {
                    [difficultyKey] = questions.Select((q, index) => new
                    {
                        questionNum = $"Question {index + 1}",
                        questionText = EncodeForJavaScript(FormatTextForDisplay(q.QuestionText)),
                        options = allOptions.Where(o => o.QuestionId == q.QuestionId).Select(o => EncodeForJavaScript(FormatTextForDisplay(o.OptionText))).ToList(),
                        correctAnswer = allOptions.Where(o => o.QuestionId == q.QuestionId && o.OptionIsCorrect).Select(o => EncodeForJavaScript(FormatTextForDisplay(o.OptionText))).FirstOrDefault(),
                        answeredCorrectly = false
                    }).ToList<object>()
                };

                return Json(quizData);
            }
        }

        private string EncodeForJavaScript(string input)
        {
            return input
                .Replace("\\", "") // Escape backslashes
                .Replace("\"", "") // Replace double quotes with HTML entities
                .Replace("'", "") // Replace single quotes with HTML entities
                .Replace("\n", " ") // Escape newlines
                .Replace("\r", " ") // Escape carriage returns

                ;
        }
        private string FormatTextForDisplay(string text)
        {
            // Échappez les guillemets et remplacez-les par des entités HTML ou des balises
            return text.Replace("\"", " ").Replace("'", " ");
        }



        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Simple()
        {
            return View();
        }
        public IActionResult Callquiz()
        {
            return View();
        }

        public IActionResult Multiple()
        {
            return View();
        }
    }
}
