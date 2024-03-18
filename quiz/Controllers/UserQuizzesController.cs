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
    public class UserQuizzesController : Controller
    {
        private readonly QuizContext _context;

        public UserQuizzesController(QuizContext context)
        {
            _context = context;
        }

        // GET: UserQuizzes
        public async Task<IActionResult> Index()
        {
              return _context.UserQuizzes != null ? 
                          View(await _context.UserQuizzes.ToListAsync()) :
                          Problem("Entity set 'QuizContext.UserQuizzes'  is null.");
        }

        // GET: UserQuizzes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.UserQuizzes == null)
            {
                return NotFound();
            }

            var userQuiz = await _context.UserQuizzes
                .FirstOrDefaultAsync(m => m.UserQuizId == id);
            if (userQuiz == null)
            {
                return NotFound();
            }

            return View(userQuiz);
        }

        // GET: UserQuizzes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UserQuizzes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserQuizId,UserQuizUsername,UserQuizPassword,UserQuizEmail,UserQuizIsAdmin,UserQuizIsActive")] UserQuiz userQuiz)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userQuiz);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userQuiz);
        }

        // GET: UserQuizzes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UserQuizzes == null)
            {
                return NotFound();
            }

            var userQuiz = await _context.UserQuizzes.FindAsync(id);
            if (userQuiz == null)
            {
                return NotFound();
            }
            return View(userQuiz);
        }

        // POST: UserQuizzes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserQuizId,UserQuizUsername,UserQuizPassword,UserQuizEmail,UserQuizIsAdmin,UserQuizIsActive")] UserQuiz userQuiz)
        {
            if (id != userQuiz.UserQuizId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userQuiz);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserQuizExists(userQuiz.UserQuizId))
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
            return View(userQuiz);
        }

        // GET: UserQuizzes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UserQuizzes == null)
            {
                return NotFound();
            }

            var userQuiz = await _context.UserQuizzes
                .FirstOrDefaultAsync(m => m.UserQuizId == id);
            if (userQuiz == null)
            {
                return NotFound();
            }

            return View(userQuiz);
        }

        // POST: UserQuizzes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UserQuizzes == null)
            {
                return Problem("Entity set 'QuizContext.UserQuizzes'  is null.");
            }
            var userQuiz = await _context.UserQuizzes.FindAsync(id);
            if (userQuiz != null)
            {
                _context.UserQuizzes.Remove(userQuiz);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserQuizExists(int id)
        {
          return (_context.UserQuizzes?.Any(e => e.UserQuizId == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> GetAllQuizzes()
        {
            if (_context.UserQuizzes == null)
            {
                return Json(new List<UserQuiz>());
            }

            var quizzes = await _context.UserQuizzes.ToListAsync();
            return Json(quizzes);
        }
        //Fonctionalite de connexion d'un user sans compte a la session multijoueur
        public IActionResult ConnectShow()
        {

            return View("~/Views/MultiUser/ConnectNoRegisterUser.cshtml");
        }

        [HttpPost]
        public IActionResult ConnectShowed()
        {
            var username = HttpContext.Request.Form["username"].ToString();
            if (string.IsNullOrWhiteSpace(username))
            {
                TempData["ErrorMessage"] = "Veuillez saisir un non d'utilisateur.";
            }
            else
            {
                TempData["SuccessMessage"] = $"Hello,{username}";
                var existingUser = _context.UserQuizzes.FirstOrDefault(u => u.UserQuizUsername == username);
                if (existingUser != null)
                {
                    TempData["ErrorMessage"] = "Ce nom d'utilisateur existe déjà.";
                    TempData["SuccessMessage"] = "";
                    return View("~/Views/MultiUser/ConnectNoRegisterUser.cshtml");
                }
                else
                {
                    TempData["SuccessMessage"] = $"Bonjour, {username}";
                    TempData["ErrorMessage"] = "";
                    return View("~/Views/MultiUser/ConnectNoRegisterUser.cshtml");
                }
            }
            return View("~/Views/MultiUser/ConnectNoRegisterUser.cshtml");
        }
    }
}