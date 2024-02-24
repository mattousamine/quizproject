using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using quiz.DAL;
using quiz.Models;

using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Text.Json;


namespace quiz.Controllers
{
    public class QuizzesController : Controller
    {
        private readonly QuizContext _context;

        public QuizzesController(QuizContext context)
        {
            _context = context;
        }

        // GET: Quizzes
        public async Task<IActionResult> Index()
        {
            return _context.Quizzes != null ?
                        View(await _context.Quizzes.ToListAsync()) :
                        Problem("Entity set 'QuizContext.Quizzes'  is null.");
        }

        // GET: Quizzes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Quizzes == null)
            {
                return NotFound();
            }

            var quiz = await _context.Quizzes.FirstOrDefaultAsync(m => m.QuizId == id);
            if (quiz == null)
            {
                return NotFound();
            }

            // Find the associated CategoryQuiz
            var categoryQuiz = await _context.CategoryQuizzes.FirstOrDefaultAsync(cq => cq.QuizId == id);
            if (categoryQuiz != null)
            {
                // Retrieve the Category name and pass it to the view via ViewBag
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == categoryQuiz.CategoryId);
                ViewBag.CategoryName = category?.CategoryName ?? "No Category";
            }
            else
            {
                ViewBag.CategoryName = "No Category";
            }

            return View(quiz);
        }

        // GET: Quizzes/Create
        public IActionResult Create()
        {
            if (!_context.Categories.Any())
            {
                ViewBag.catErrorMessage = "You must create a category in order to create a quiz.";
            }
            else
            {
                ViewBag.catErrorMessage = "";
            }

            ViewBag.CategoryId = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Quizzes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("QuizId,QuizName,QuizIsActive, CategoryId")] Quiz quiz, int CategoryId)
        {
            if (ModelState.IsValid)
            {
                _context.Add(quiz);
                await _context.SaveChangesAsync();

                // Check if the category is already assigned to the quiz
                if (!_context.CategoryQuizzes.Any(cq => cq.QuizId == quiz.QuizId && cq.CategoryId == CategoryId))
                {
                    var categoryQuiz = new CategoryQuiz { QuizId = quiz.QuizId, CategoryId = CategoryId };
                    _context.Add(categoryQuiz);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CategoryId = new SelectList(_context.Categories, "CategoryId", "CategoryName", CategoryId);
            return View(quiz);
        }

        // GET: Quizzes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Quizzes == null)
            {
                return NotFound();
            }

            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null)
            {
                return NotFound();
            }

            var quizImg = await _context.QuizImg.FirstOrDefaultAsync(img => img.QuizId == id);
            ViewBag.QuizImgUrl = quizImg?.Img;

            var categoryQuiz = await _context.CategoryQuizzes.FirstOrDefaultAsync(cq => cq.QuizId == id);
            ViewBag.SelectedCategoryId = categoryQuiz?.CategoryId;

            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "CategoryId", "CategoryName");
            return View(quiz);
        }

        // POST: Quizzes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("QuizId,QuizName,QuizIsActive")] Quiz quiz, int CategoryId, IFormFile quizImg)
        {

            if (id != quiz.QuizId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (quizImg != null && quizImg.Length > 0)
                    {
                        // Upload new photo and update QuizImg
                        var imgUrl = await UploadToImgur(quizImg); // Implement this method based on your requirements
                        var existingQuizImg = await _context.QuizImg.FirstOrDefaultAsync(img => img.QuizId == quiz.QuizId);
                        if (existingQuizImg != null)
                        {
                            existingQuizImg.Img = imgUrl;
                        }
                        else
                        {
                            var newQuizImg = new QuizImg { QuizId = quiz.QuizId, Img = imgUrl };
                            _context.QuizImg.Add(newQuizImg);
                        }
                    }
                    _context.Update(quiz);
                    await _context.SaveChangesAsync();

                    // Handle the category change here
                    var categoryQuiz = await _context.CategoryQuizzes.FirstOrDefaultAsync(cq => cq.QuizId == quiz.QuizId);
                    if (categoryQuiz != null)
                    {
                        categoryQuiz.CategoryId = CategoryId;
                        _context.Update(categoryQuiz);
                    }
                    else
                    {
                        // If there was no entry in CategoryQuizzes, create one
                        var newCategoryQuiz = new CategoryQuiz { QuizId = quiz.QuizId, CategoryId = CategoryId };
                        _context.CategoryQuizzes.Add(newCategoryQuiz);
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuizExists(quiz.QuizId))
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

            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "CategoryId", "CategoryName", CategoryId);
            return View(quiz);
        }


        // GET: Quizzes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Quizzes == null)
            {
                return NotFound();
            }

            var quiz = await _context.Quizzes
                .FirstOrDefaultAsync(m => m.QuizId == id);
            if (quiz == null)
            {
                return NotFound();
            }

            return View(quiz);
        }

        // POST: Quizzes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null)
            {
                return NotFound();
            }

            // Find and remove all associated CategoryQuiz entries
            var categoryQuizzes = _context.CategoryQuizzes.Where(cq => cq.QuizId == id);
            _context.CategoryQuizzes.RemoveRange(categoryQuizzes);

            // Remove the quiz
            _context.Quizzes.Remove(quiz);

            // Save the changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> userHomePage(string searchTerm=" ")
        {
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim(); // Trim whitespaces
                var categoriesWithQuizzes = await _context.Categories
                    .Select(category => new
                    {
                        category.CategoryId,
                        category.CategoryName,
                        Quizzes = _context.CategoryQuizzes
                            .Where(cq => cq.CategoryId == category.CategoryId)
                            .Where(cq => cq.Quiz.QuizName.Contains(searchTerm))
                            .Select(cq => new
                            {
                                cq.Quiz.QuizId,
                                cq.Quiz.QuizName,
                                QuizImg = _context.QuizImg
                                    .Where(qi => qi.QuizId == cq.QuizId)
                                    .Select(qi => qi.Img)
                                    .FirstOrDefault() ?? "https://i.imgur.com/BexKsap.png"
                            }).ToList()
                    }).ToListAsync();

                ViewBag.CategoriesWithQuizzes = categoriesWithQuizzes;
            }
            else
            {
                // Handle case when searchTerm is null or empty
                // You might want to show all quizzes in this case
                var allCategoriesWithQuizzes = await _context.Categories
                    .Select(category => new
                    {
                        category.CategoryId,
                        category.CategoryName,
                        Quizzes = _context.CategoryQuizzes
                            .Where(cq => cq.CategoryId == category.CategoryId)
                            .Select(cq => new
                            {
                                cq.Quiz.QuizId,
                                cq.Quiz.QuizName,
                                QuizImg = _context.QuizImg
                                    .Where(qi => qi.QuizId == cq.QuizId)
                                    .Select(qi => qi.Img)
                                    .FirstOrDefault() ?? "https://i.imgur.com/BexKsap.png"
                            }).ToList()
                    }).ToListAsync();

                ViewBag.CategoriesWithQuizzes = allCategoriesWithQuizzes;
            }

            return View();
        }




        private bool QuizExists(int id)
        {
            return (_context.Quizzes?.Any(e => e.QuizId == id)).GetValueOrDefault();
        }

        private async Task<string> UploadToImgur(IFormFile imageFile)
        {
            string imageUrl = "";
            var clientID = "b7ec07bb1929918"; 
            var imgurApiUrl = "https://api.imgur.com/3/image";

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Client-ID", clientID);

                using (var content = new MultipartFormDataContent())
                {
                    using (var imageStream = imageFile.OpenReadStream())
                    {
                        content.Add(new StreamContent(imageStream), "image");
                        var response = await httpClient.PostAsync(imgurApiUrl, content);

                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            var parsedJson = JsonDocument.Parse(data);
                            imageUrl = parsedJson.RootElement.GetProperty("data").GetProperty("link").GetString();
                        }
                    }
                }
            }

            return imageUrl;
        }

    }
}
