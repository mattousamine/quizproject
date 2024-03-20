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

        public ActionResult MultiplayerQuiz(int quizid, int multiplayer)
        {

            ViewBag.QuizId = quizid;

            return View();
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


        public ActionResult ShowUserScore()
        {
            return View("~/Views/Score/UserScore.cshtml");
        }
        public ActionResult ShowAdminScore()
        {
            return View("~/Views/Score/AdminScore.cshtml");
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
        public async Task<IActionResult> SaveQuizSession(int quizId, int score, string userSession = "")
        {
            if (!string.IsNullOrEmpty(userSession))
            {
                // Handle multiplayer session case
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Check if the quiz exists
                        var quizExists = await _context.Quizzes.AnyAsync(q => q.QuizId == quizId);
                        if (!quizExists)
                        {
                            return Json(new { ok = false, message = "Quiz does not exist." });
                        }

                        // Create or update the multiplayer session
                        var session = await _context.MultiplayerTmpSession
                            .FirstOrDefaultAsync(m => m.Username == userSession && m.QuizId == quizId);

                        if (session == null)
                        {
                            // If session does not exist, create a new one
                            session = new MultiplayerTmpSession
                            {
                                Username = userSession,
                                QuizId = quizId,
                                Score = score
                            };
                            await _context.MultiplayerTmpSession.AddAsync(session);
                        }
                        else
                        {
                            // If session exists, update the score
                            session.Score = score;
                        }

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return Json(new { ok = true });
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        Console.WriteLine(ex.ToString());
                        return StatusCode(500, "An error occurred while processing your request.");
                    }
                }
            }
            else
            {
                // Fetch the userId from session or another state management mechanism
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    return Unauthorized();
                }

                // Creating a new QuizSession instance with the current time
                var quizSession = new QuizSession
                {
                    QuizSessionScore = score,
                    QuizSessionTime = DateTime.Now, // Use the current time when saving the session
                };

                _context.QuizSessions.Add(quizSession);
                await _context.SaveChangesAsync();

                // Check if a QuizTracker with the same userId and quizId already exists
                var existingTracker = await _context.QuizTrackers
                    .FirstOrDefaultAsync(qt => qt.UserQuizId == userId.Value && qt.QuizId == quizId);

                if (existingTracker != null)
                {
                    // If found, update the existing tracker entry
                    existingTracker.QuizSessionId = quizSession.QuizSessionId;
                }
                else
                {
                    // If not found, create a new QuizTracker entry
                    var quizTracker = new QuizTracker
                    {
                        UserQuizId = userId.Value,
                        QuizSessionId = quizSession.QuizSessionId,
                        QuizId = quizId
                    };

                    _context.QuizTrackers.Add(quizTracker);
                }

                await _context.SaveChangesAsync();

                return Ok();
            }
        }

        public async Task<IActionResult> GetAdminScoreStats()
        {
            var quizTrackers = _context.QuizTrackers.ToList();
            var userQuizzes = _context.UserQuizzes.ToList();
            var quizzes = _context.Quizzes.ToList();
            var quizSessions = _context.QuizSessions.ToList();

            var stats = (from qt in quizTrackers
                         join uq in userQuizzes on qt.UserQuizId equals uq.UserQuizId
                         join q in quizzes on qt.QuizId equals q.QuizId
                         join qs in quizSessions on qt.QuizSessionId equals qs.QuizSessionId
                         select new
                         {
                             User = uq.UserQuizUsername,
                             Quiz = q.QuizName,
                             Date = qs.QuizSessionTime.ToString("o"),
                             Score = qs.QuizSessionScore
                         }).ToList();

            return Json(stats);
        }

        public async Task<IActionResult> GetUserQuizScores()
        {
            try
            {
                int currentUserId = GetCurrentUserId();
                if (currentUserId == 0)
                {
                    return Unauthorized();
                }

                var quizScores = await (from qt in _context.QuizTrackers
                                        where qt.UserQuizId == currentUserId
                                        join q in _context.Quizzes on qt.QuizId equals q.QuizId
                                        join qs in _context.QuizSessions on qt.QuizSessionId equals qs.QuizSessionId
                                        select new
                                        {
                                            QuizName = q.QuizName,
                                            Score = qs.QuizSessionScore // Assuming the score is already scaled to a max of 100
                                        }).ToListAsync();

                return Json(quizScores);
            }
            catch (Exception ex)
            {
                // Log the exception message or handle it as required
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }


        private int GetCurrentUserId()
        {
            // Retrieve the user ID from the current session or context
            // This is a placeholder, implement this according to your authentication mechanism
            return HttpContext.Session.GetInt32("UserId") ?? 0;
        }

        [HttpGet]
        public async Task<IActionResult> CheckUsernameAvailability(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return Json(false);
            }

            try
            {
                // Ensuring the operation is as efficient as possible
                var isUsernameTaken = await _context.MultiplayerTmpSession
                                        .AnyAsync(m => EF.Functions.Like(m.Username, username));


                // Return true if the username is NOT taken (i.e., available)
                return Json(!isUsernameTaken);
            }
            catch (Exception ex)
            {

                // Optionally log the exception
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SubscribeQuizTmp(string username, int quizId)
        {
            if (string.IsNullOrWhiteSpace(username) || quizId <= 0)
            {
                return Json(new { ok = false, message = "Invalid username or quiz ID." });
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Check if the quiz exists
                    var quizExists = await _context.Quizzes.AnyAsync(q => q.QuizId == quizId);
                    if (!quizExists)
                    {
                        return Json(new { ok = false, message = "Quiz does not exist." });
                    }

                    // Create a new session
                    var newSession = new MultiplayerTmpSession
                    {
                        Username = username,
                        QuizId = quizId,
                        Score = 0
                    };

                    // Save the new session
                    await _context.MultiplayerTmpSession.AddAsync(newSession);
                    await _context.SaveChangesAsync();

                    // Commit transaction
                    await transaction.CommitAsync();

                    return Json(new { ok = true });
                }
                catch (DbUpdateException ex)
                {
                    // Log the exception
                    Console.WriteLine(ex.ToString());

                    // This could be due to a variety of reasons, including duplicate usernames for the same quiz
                    await transaction.RollbackAsync();
                    return Json(new { ok = false, message = ex.ToString() });
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine(ex.ToString());

                    await transaction.RollbackAsync();
                    return StatusCode(500, ex.ToString());
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetQuizDates(int quizId)
        {
            try
            {
                var quizTimes = await _context.MultiPlayerQuiz
                    .Where(q => q.QuizId == quizId)
                    .Select(q => new { q.BeginDate, q.EndDate })
                    .FirstOrDefaultAsync();

                if (quizTimes == null)
                {
                    return NotFound("Quiz not found.");
                }

                return Json(new
                {
                    beginDate = quizTimes.BeginDate.ToString("o"), // ISO 8601 format
                    endDate = quizTimes.EndDate.ToString("o")
                });
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

    }
}
