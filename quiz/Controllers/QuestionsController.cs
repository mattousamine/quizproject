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
    public class QuestionsController : Controller
    {
        private readonly QuizContext _context;

        public QuestionsController(QuizContext context)
        {
            _context = context;
        }

        // GET: Questions
        public async Task<IActionResult> Index()
        {
              return _context.Questions != null ? 
                          View(await _context.Questions.ToListAsync()) :
                          Problem("Entity set 'QuizContext.Questions'  is null.");
        }

        // GET: Questions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Questions == null)
            {
                return NotFound();
            }

            var question = await _context.Questions
                .FirstOrDefaultAsync(m => m.QuestionId == id);
            if (question == null)
            {
                return NotFound();
            }
            // Find the quiz IDs associated with this question
            var associatedQuizIds = await _context.QuizQuestions
                .Where(qq => qq.QuestionId == id)
                .Select(qq => qq.QuizId)
                .ToListAsync();

            // If you need to get quiz names or other properties, you can query the Quizzes DbSet
            // For example, to get quiz names:
            var associatedQuizNames = await _context.Quizzes
                .Where(q => associatedQuizIds.Contains(q.QuizId))
                .Select(q => q.QuizName)
                .ToListAsync();

            // Put the associated quiz IDs or quiz names in the ViewBag
            ViewBag.AssociatedQuizIds = associatedQuizIds;  // For IDs
            ViewBag.AssociatedQuizNames = associatedQuizNames;  // For names

            return View(question);
        }

        // GET: Questions/Create
        public IActionResult Create()
        {
            var quizzes = _context.Quizzes.ToList();
            if (quizzes.Any())
            {
                ViewBag.Quizzes = new SelectList(quizzes, "QuizId", "QuizName");
                ViewBag.QuizzesExist = true;
            }
            else
            {
                ViewBag.QuizzesExist = false;
                ViewBag.ErrorMessage = "You must create a quiz before creating a question.";
            }
            return View();
        }

        // POST: Questions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("QuestionId,QuestionText,QuestionFeedback,QuestionLevel,QuestionIsActive")] Question question, int[] selectedQuizzes)
        {
            if (ModelState.IsValid)
            {
                _context.Add(question);
                await _context.SaveChangesAsync();

                // After saving the question, add connections to selected quizzes
                foreach (var quizId in selectedQuizzes)
                {
                    _context.QuizQuestions.Add(new QuizQuestion { QuestionId = question.QuestionId, QuizId = quizId });
                }
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(question);
        }

        // GET: Questions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Questions == null)
            {
                return NotFound();
            }

            var question = await _context.Questions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }
            // Get a list of all quizzes to allow the user to choose which quizzes this question belongs to
            ViewBag.AllQuizzes = new SelectList(await _context.Quizzes.ToListAsync(), "QuizId", "QuizName");

            // Find out which quizzes the question is currently associated with
            ViewBag.SelectedQuizzes = await _context.QuizQuestions
                .Where(qq => qq.QuestionId == id)
                .Select(qq => qq.QuizId)
                .ToListAsync();

            return View(question);
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("QuestionId,QuestionText,QuestionFeedback,QuestionLevel,QuestionIsActive")] Question question, int[] selectedQuizzes)
        {
            if (id != question.QuestionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update the question itself
                    _context.Update(question);
                    await _context.SaveChangesAsync();

                    // Get the current quiz-question connections for this question
                    var currentQuizQuestions = _context.QuizQuestions.Where(qq => qq.QuestionId == id);

                    // Remove the old connections
                    _context.QuizQuestions.RemoveRange(currentQuizQuestions);

                    // Add the new connections based on the user's selection
                    foreach (var quizId in selectedQuizzes)
                    {
                        _context.QuizQuestions.Add(new QuizQuestion { QuestionId = id, QuizId = quizId });
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(question.QuestionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // If we got this far, something failed, so reload the data for the view
            ViewBag.AllQuizzes = new SelectList(await _context.Quizzes.ToListAsync(), "QuizId", "QuizName");
            ViewBag.SelectedQuizzes = await _context.QuizQuestions
                .Where(qq => qq.QuestionId == id)
                .Select(qq => qq.QuizId)
                .ToListAsync();

            return View(question);
        }

        // GET: Questions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Questions == null)
            {
                return NotFound();
            }

            var question = await _context.Questions
                .FirstOrDefaultAsync(m => m.QuestionId == id);
            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Questions == null)
            {
                return Problem("Entity set 'QuizContext.Questions'  is null.");
            }
            var question = await _context.Questions.FindAsync(id);
            if (question != null)
            {
                var quizQuestions = _context.QuizQuestions.Where(qq => qq.QuestionId == id);
                _context.QuizQuestions.RemoveRange(quizQuestions);

                _context.Questions.Remove(question);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuestionExists(int id)
        {
          return (_context.Questions?.Any(e => e.QuestionId == id)).GetValueOrDefault();
        }
    }
}
