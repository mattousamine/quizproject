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
    public class OptionsController : Controller
    {
        private readonly QuizContext _context;

        public OptionsController(QuizContext context)
        {
            _context = context;
        }

        // GET: Options
        public async Task<IActionResult> Index()
        {
              return _context.Options != null ? 
                          View(await _context.Options.ToListAsync()) :
                          Problem("Entity set 'QuizContext.Options'  is null.");
        }

        // GET: Options/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Options == null)
            {
                return NotFound();
            }

            var option = await _context.Options.FirstOrDefaultAsync(m => m.OptionId == id);
            if (option == null)
            {
                return NotFound();
            }

            // Query the QuestionOptions to find the associated question ID
            var questionOption = await _context.QuestionOptions
                .Where(qo => qo.OptionId == id)
                .FirstOrDefaultAsync();

            if (questionOption != null)
            {
                // Retrieve the associated Question details
                var question = await _context.Questions
                    .Where(q => q.QuestionId == questionOption.QuestionId)
                    .FirstOrDefaultAsync();

                if (question != null)
                {
                    // Pass the associated Question details to the view
                    ViewBag.AssociatedQuestionText = question.QuestionText;
                    ViewBag.AssociatedQuestionId = question.QuestionId;
                }
            }

            return View(option);
        }

        // GET: Options/Create
        public IActionResult Create()
        {
            ViewBag.QuestionId = new SelectList(_context.Questions, "QuestionId", "QuestionText");
            return View();
        }

        // POST: Options/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OptionId,OptionText,OptionIsCorrect")] Option option, int questionId)
        {
            if (ModelState.IsValid)
            {
                _context.Options.Add(option);
                await _context.SaveChangesAsync();

                // Assuming OptionId is set automatically by the database upon saving the option
                var questionOption = new QuestionOption
                {
                    QuestionId = questionId,
                    OptionId = option.OptionId
                };

                _context.QuestionOptions.Add(questionOption);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // If we get here, something was wrong with the form, so send back the form with the list of questions
            ViewBag.QuestionId = new SelectList(_context.Questions, "QuestionId", "QuestionText", questionId);
            return View(option);
        }


        // GET: Options/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Options == null)
            {
                return NotFound();
            }

            var option = await _context.Options.FindAsync(id);
            if (option == null)
            {
                return NotFound();
            }

            // Find the current associated question for the option
            var questionOption = await _context.QuestionOptions
                .FirstOrDefaultAsync(qo => qo.OptionId == id);
            ViewBag.CurrentQuestionId = questionOption?.QuestionId;

            // Provide a list of questions for dropdown
            ViewBag.QuestionId = new SelectList(_context.Questions, "QuestionId", "QuestionText", ViewBag.CurrentQuestionId);

            return View(option);
        }


        // POST: Options/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OptionId,OptionText,OptionIsCorrect")] Option option, int QuestionId)
        {
            if (id != option.OptionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(option);
                    await _context.SaveChangesAsync();

                    // Update the question-option association
                    var questionOption = await _context.QuestionOptions
                        .FirstOrDefaultAsync(qo => qo.OptionId == option.OptionId);
                    if (questionOption != null)
                    {
                        questionOption.QuestionId = QuestionId; // Update association
                        _context.Update(questionOption);
                    }
                    else
                    {
                        // If there was no previous association, create a new one
                        _context.QuestionOptions.Add(new QuestionOption { OptionId = option.OptionId, QuestionId = QuestionId });
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OptionExists(option.OptionId))
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

            // Repopulate ViewBag if returning to view due to validation error
            ViewBag.QuestionId = new SelectList(_context.Questions, "QuestionId", "QuestionText", QuestionId);

            return View(option);
        }


        // GET: Options/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Options == null)
            {
                return NotFound();
            }

            var option = await _context.Options
                .FirstOrDefaultAsync(m => m.OptionId == id);
            if (option == null)
            {
                return NotFound();
            }

            return View(option);
        }

        // POST: Options/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Options == null)
            {
                return Problem("Entity set 'QuizContext.Options'  is null.");
            }

            // Find any QuestionOptions associations for the option
            var questionOptions = await _context.QuestionOptions
                .Where(qo => qo.OptionId == id)
                .ToListAsync();

            // If any associations exist, remove them
            if (questionOptions.Any())
            {
                _context.QuestionOptions.RemoveRange(questionOptions);
                await _context.SaveChangesAsync(); // Ensure associations are removed before deleting the option
            }

            var option = await _context.Options.FindAsync(id);
            if (option != null)
            {
                _context.Options.Remove(option);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        private bool OptionExists(int id)
        {
          return (_context.Options?.Any(e => e.OptionId == id)).GetValueOrDefault();
        }
    }
}
