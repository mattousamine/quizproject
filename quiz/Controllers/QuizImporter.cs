using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using quiz.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using quiz.DAL;
using Microsoft.EntityFrameworkCore;

namespace quiz.Controllers
{
    public class QuizImporter : Controller
    {
        private readonly QuizContext _context;
        public QuizImporter(QuizContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View("~/Views/ExcelImporter/UploadExcel.cshtml");
        }

        [HttpPost]
        public ActionResult Import(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Please upload a file.");
                return View("~/Views/ExcelImporter/UploadExcel.cshtml");
            }

            var excelData = new List<QuizImport>();
            var missingColumns = new List<string>();
            var rowErrors = new List<string>();
            bool allColumnsExist = true;

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheets.First();
                    var requiredColumns = new[] { "Category", "Quiz", "Questions", "Option1", "Option2", "Option3", "Option4", "Option5", "Correct_Answer", "Feedback_Text", "Level" };

                    // Check for required columns
                    foreach (var reqColumn in requiredColumns)
                    {
                        if (!worksheet.Row(1).CellsUsed().Any(c => c.Value.ToString().Trim().ToLower() == reqColumn.ToLower()))
                        {
                            allColumnsExist = false;
                            missingColumns.Add(reqColumn);
                        }
                    }

                    if (!allColumnsExist)
                    {
                        string missingColumnsMessage = "Missing columns: " + string.Join(", ", missingColumns);
                        ViewBag.RowErrors = new List<string> { missingColumnsMessage };
                        return View("~/Views/ExcelImporter/UploadExcel.cshtml");
                    }

                    var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header row
                    int rowNumber = 2; // Start from second row, as first row is headers

                    foreach (var row in rows)
                    {
                        var newQuizImport = new QuizImport()
                        {
                            Category = row.Cell("A").GetValue<string>(),
                            Quiz = row.Cell("B").GetValue<string>(),
                            Questions = row.Cell("C").GetValue<string>(),
                            Option1 = row.Cell("D").GetValue<string>(),
                            Option2 = row.Cell("E").GetValue<string>(),
                            Option3 = row.Cell("F").GetValue<string>(),
                            Option4 = row.Cell("G").GetValue<string>(),
                            Option5 = row.Cell("H").GetValue<string>(),
                            CorrectAnswer = row.Cell("I").GetValue<string>(),
                            FeedbackText = row.Cell("J").GetValue<string>(),
                            Level = ParseLevel(row.Cell("K").GetValue<string>())
                        };

                        List<string> errorMessages;
                        if (!IsRowValid(newQuizImport, out errorMessages))
                        {
                            rowErrors.AddRange(errorMessages.Select(e => $"Row {rowNumber}: {e}"));
                            rowNumber++;
                            continue;
                        }

                        excelData.Add(newQuizImport);
                        rowNumber++;
                    }
                }
            }

            if (rowErrors.Any())
            {
                ViewBag.RowErrors = rowErrors;
            }

            return View("~/Views/ExcelImporter/PreviewImport.cshtml", excelData);
        }

        

        private bool IsRowValid(QuizImport quizImport, out List<string> errorMessages)
        {
            errorMessages = new List<string>();

            if (string.IsNullOrWhiteSpace(quizImport.Category))
                errorMessages.Add("Category cannot be empty.");

            if (string.IsNullOrWhiteSpace(quizImport.Quiz))
                errorMessages.Add("Quiz cannot be empty.");

            if (string.IsNullOrWhiteSpace(quizImport.Questions))
                errorMessages.Add("Questions cannot be empty.");

            if (string.IsNullOrWhiteSpace(quizImport.Option1))
                errorMessages.Add("Option1 cannot be empty.");

            if (string.IsNullOrWhiteSpace(quizImport.Option2))
                errorMessages.Add("Option2 cannot be empty.");

            if (string.IsNullOrWhiteSpace(quizImport.Option3))
                errorMessages.Add("Option3 cannot be empty.");

            if (string.IsNullOrWhiteSpace(quizImport.Option4))
                errorMessages.Add("Option4 cannot be empty.");

            if (string.IsNullOrWhiteSpace(quizImport.Option5))
                errorMessages.Add("Option5 cannot be empty.");

            if (string.IsNullOrWhiteSpace(quizImport.CorrectAnswer))
            {
                errorMessages.Add("CorrectAnswer cannot be empty.");
            }
            else
            {
                // Ensure that CorrectAnswer is a string representing either a single digit or a sequence of digits separated by dots.
                var correctAnswerParts = quizImport.CorrectAnswer.Split('.');
                if (!correctAnswerParts.All(part => part.All(char.IsDigit)))
                {
                    errorMessages.Add("CorrectAnswer must be a number or a series of numbers separated by dots.");
                }
            }

            if (string.IsNullOrWhiteSpace(quizImport.FeedbackText))
                errorMessages.Add("FeedbackText cannot be empty.");

            // The Level should only be 1, 2, or 3. If it's not one of these, it defaults to 2.
            if (quizImport.Level < 1 || quizImport.Level > 3)
                errorMessages.Add("Level must be 1, 2, or 3.");

            return !errorMessages.Any();
        }


        private int ParseLevel(string levelString)
        {
            // Parse the level and return an integer, defaulting to 2 if invalid.
            return int.TryParse(levelString, out int level) && (level == 1 || level == 2 || level == 3) ? level : 1;
        }

        [HttpPost]
        public ActionResult FinalImport(List<QuizImport> quizImports)
        {
            if (quizImports == null || !quizImports.Any())
            {
                // Handle the case where no data is passed
                return View("~/Views/ExcelImporter/UploadExcel.cshtml");
            }

            foreach (var import in quizImports)
            {
                // Check if Category exists, if not create it

                // Normalize the category name for consistent comparison
                var normalizedCategoryName = import.Category.Trim().ToLower();

                // Check if Category exists, if not create it
                var category = _context.Categories
                    .FirstOrDefault(c => c.CategoryName.Trim().ToLower() == normalizedCategoryName);

                if (category == null)
                {
                    category = new Category { CategoryName = import.Category, CategoryIsActive = true };
                    _context.Categories.Add(category);
                    // Save changes here if you need the ID of the category immediately
                    _context.SaveChanges();
                }


                // Check if Quiz exists, if not create it
                var normalizedQuizName = import.Quiz.Trim().ToLower();
                var quiz = _context.Quizzes
                    .FirstOrDefault(q => q.QuizName.Trim().ToLower() == normalizedQuizName);

                if (quiz == null)
                {
                    quiz = new Quiz { QuizName = import.Quiz, QuizIsActive = true };
                    _context.Quizzes.Add(quiz);
                    // Save changes here if you need the ID of the quiz immediately
                    _context.SaveChanges();
                }

                // Continue with adding questions and options...


                // Add to CategoryQuiz if not exists
                if (!_context.CategoryQuizzes.Any(cq => cq.CategoryId == category.CategoryId && cq.QuizId == quiz.QuizId))
                {
                    _context.CategoryQuizzes.Add(new CategoryQuiz { CategoryId = category.CategoryId, QuizId = quiz.QuizId });
                }

                // Add Question
               
                var question = new Question
                {
                    QuestionText = import.Questions,
                    QuestionFeedback = import.FeedbackText,
                    QuestionLevel = (short)import.Level,
                    QuestionIsActive = true
                };

                _context.Questions.Add(question);
                // Save the question to the database to generate the QuestionId
                _context.SaveChanges();

                // Now the QuestionId is available in the 'question' entity after the save operation.
                // Add to QuizQuestion
                var quizQuestion = new QuizQuestion { QuizId = quiz.QuizId, QuestionId = question.QuestionId };
                _context.QuizQuestions.Add(quizQuestion);

                // Process Options
                var correctAnswers = import.CorrectAnswer.Split('.').Select(int.Parse).ToList();
                var optionsText = new[] { import.Option1, import.Option2, import.Option3, import.Option4, import.Option5 };
                List<Option> optionsForThisQuestion = new List<Option>();


                for (int i = 0; i < optionsText.Length; i++)
                {
                    if (optionsText[i].ToLower() != "na")
                    {
                        var option = new Option
                        {
                            OptionText = optionsText[i],
                            OptionIsCorrect = correctAnswers.Contains(i + 1)
                        };

                        optionsForThisQuestion.Add(option);
                        _context.Options.Add(option);
                    }
                }

                // Save changes to generate OptionIds for all added options
                _context.SaveChanges();

                // Now, associate options with the question
                foreach (var option in optionsForThisQuestion) // This considers only options added in the current context
                {
                    _context.QuestionOptions.Add(new QuestionOption { QuestionId = question.QuestionId, OptionId = option.OptionId });
                }

                // Optionally, save changes again if needed immediately after each option addition
                _context.SaveChanges();
            }

            // Save all changes to the database
            _context.SaveChanges();

            // Redirect to a confirmation page or back to the list
            return View("~/Views/ExcelImporter/ImportConfirmation.cshtml");
        }



    }
}
