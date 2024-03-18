using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quiz.DAL; 
using quiz.Models;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace quiz.Controllers
{
    public class AdminController : Controller
    {
        private readonly QuizContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public AdminController(QuizContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
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

        [HttpGet]
        public async Task<IActionResult> MultiPlayerForm()
        {
            var quizzes = await _context.Quizzes
                .Where(q => q.QuizIsActive)
                .Select(q => new { q.QuizId, q.QuizName })
                .ToListAsync();

            return View("~/Views/Admin/MultiPlayerForm.cshtml", quizzes);
        }


        [HttpPost]
        public async Task<IActionResult> ProcessMultiplayerQuiz(int quizId, DateTime beginDateTime, DateTime endDateTime)
        {
            string scheme = Request.Scheme;
            string host = Request.Host.Value;
            string path = Url.Action("MultiplayerQuiz", "Quiz", new { quizid = quizId, multiplayer = 1 });

            string fullUrl = $"{scheme}://{host}{path}";
            string encodedQuizUrl = Uri.EscapeDataString(fullUrl);
            string qrApiUrl = $"https://api.qrserver.com/v1/create-qr-code/?data={fullUrl}&size=200x200";

            var httpClient = _httpClientFactory.CreateClient();
            string imgUrl = "";

            try
            {
                // First, get the QR code image
                var qrResponse = await httpClient.GetAsync(fullUrl);
                if (!qrResponse.IsSuccessStatusCode)
                {
                    return StatusCode((int)qrResponse.StatusCode, "Error generating QR code");
                }
                var imageContent = await qrResponse.Content.ReadAsByteArrayAsync();

                // Then, upload the image to Imgur
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Client-ID", "b7ec07bb1929918");
                var imgurUploadRequest = new MultipartFormDataContent();
                imgurUploadRequest.Add(new ByteArrayContent(imageContent), "image");

                var imgurResponse = await httpClient.PostAsync("https://api.imgur.com/3/image", imgurUploadRequest);
                if (!imgurResponse.IsSuccessStatusCode)
                {
                    return StatusCode((int)imgurResponse.StatusCode, "Error uploading image to Imgur");
                }

                var imgurResponseContent = await imgurResponse.Content.ReadAsStringAsync();
                dynamic jsonResponse = JsonConvert.DeserializeObject(imgurResponseContent);
                bool success = jsonResponse.success;
                if (success)
                {
                    imgUrl = jsonResponse.data.link;
                    return Json(new { Url = imgUrl }); 
                }
                else
                {
                    return StatusCode(500, "Imgur upload was not successful.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error while generating QR code and uploading to Imgur.");
            }
        }






    }
}
