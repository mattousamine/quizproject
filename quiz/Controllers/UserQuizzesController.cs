using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quiz.DAL;
using quiz.Models;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Drawing;
using System.Xml.Linq;
using PdfSharp.Drawing.Layout;


namespace quiz.Controllers
{
    public class UserQuizzesController : Controller
    {
        private readonly QuizContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public UserQuizzesController(QuizContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
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

        public IActionResult verifyUsername(string username)
        {
            var existingUser = _context.UserQuizzes.FirstOrDefault(u => u.UserQuizUsername == username);
            return Json(new { exists = existingUser });
        }

        public IActionResult VerifyMultiUserSessionUsername(string username)
        {
            var existingUser = _context.MultiUserSession.FirstOrDefault(u => u.Username == username);
            return Json(new { exists = existingUser });
        }

        // Méthode pour exporter les résultats d'un quiz en PDF


        public async Task<IActionResult> ExportQuizResultsToPdf(int quizId)
        {

            var sessions = await _context.MultiUserSession.Where(s => s.QuizId == quizId).ToListAsync();
            var userInfos = new List<(string Name, int Score)>();

            foreach (var session in sessions)
            {
                var user = await _context.UserQuizzes.FirstOrDefaultAsync(u => u.UserQuizId == session.UserId);
                if (user != null)
                {
                    userInfos.Add((user.UserQuizUsername, user.UserQuizScore));
                }
            }

            var pdf = new PdfDocument();
            var page = pdf.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            XFont font = new XFont("Verdana", 12, PdfSharp.Fonts.XFontStyle.Regular);
            XTextFormatter tf = new XTextFormatter(gfx);
            tf.DrawString("Résultats du Quiz", font, XBrushes.Black,
                new XRect(10, 10, page.Width, page.Height), XStringFormats.TopLeft);

            int yPos = 30;
            foreach (var userInfo in userInfos)
            {
                tf.DrawString($"Nom: {userInfo.Name}, Score: {userInfo.Score.ToString()}", font, XBrushes.Black,
                    new XRect(10, yPos, page.Width, page.Height), XStringFormats.TopLeft);
                yPos += 20;
            }

            var wwwrootPath = _hostingEnvironment.WebRootPath;
            var pdfFilename = Path.Combine(wwwrootPath, "uploads", "quiz_results.pdf");
            pdf.Save(pdfFilename);

            return File(pdfFilename, "application/pdf", "quiz_results.pdf");
        }

    }
}
