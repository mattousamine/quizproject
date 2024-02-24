using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using quiz.DAL;
using quiz.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace quiz.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly QuizContext _context;
        public AuthenticationController(QuizContext context)
        {
            _context = context;
        }
        public IActionResult VerifyEmail(string email)
        {
            bool emailExists = _context.UserQuizzes.Any(u => u.UserQuizEmail == email);
            return Json(new { exists = emailExists });
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserQuiz userQuiz)
        {
            if (ModelState.IsValid)
            {
                // Check if the email already exists
                if (_context.UserQuizzes.Any(u => u.UserQuizEmail == userQuiz.UserQuizEmail))
                {
                    // Add an error message to the ModelState
                    ModelState.AddModelError("UserQuizEmail", "Email is already taken.");
                    return View("Registre");
                }

                // Hash the password
                userQuiz.UserQuizPassword = HashPasswordSHA256(userQuiz.UserQuizPassword);

                // Set the default values for the new user
                userQuiz.UserQuizIsAdmin = false;
                userQuiz.UserQuizIsActive = true;

                // Save the new user to the database
                _context.UserQuizzes.Add(userQuiz);
                await _context.SaveChangesAsync();

                // Redirect to the Registre view with a success message
                TempData["SuccessMessage"] = "Registration successful! You can now log in.";
                return RedirectToAction("Registre", "Home");
            }

            TempData["ErrorMessage"] = "There are validation errors.";
            return RedirectToAction("Registre", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = _context.UserQuizzes.FirstOrDefault(u => u.UserQuizEmail == email);
            TempData["logOutMessage"] = null;

            if (user != null && VerifyPassword(password, user.UserQuizPassword))
            {
                // Set session variable
                HttpContext.Session.SetString("UserEmail", user.UserQuizEmail);
                HttpContext.Session.SetString("UserRole", user.UserQuizIsAdmin ? "Admin" : "User");

                TempData["SuccessMessage"] = "Login successful!";
               
                return RedirectToAction("Index", "Home");
            }

            TempData["ErrorMessage"] = "Invalid login attempt.";
            return RedirectToAction("Login", "Home");
        }

        [HttpPost]
        public IActionResult ResetPasswordRequest(string email)
        {
            var user = _context.UserQuizzes.FirstOrDefault(u => u.UserQuizEmail == email);
            if (user != null)
            {
                // For now, just redirect to a password reset page with the user's ID
                return RedirectToAction("ResetPassword", "Home", new { userId = user.UserQuizId });
            }
            else
            {
                TempData["ErrorMessage"] = "No account found with that email address.";
                return RedirectToAction("Password", "Home");  // Redirect back to the forget password page
            }
        }

   

        [HttpPost]
        public async Task<IActionResult> ChangePassword(int userId, string newPassword)
        {
            var user = _context.UserQuizzes.Find(userId);
            if (user != null)
            {
                // Hash the new password
                user.UserQuizPassword = HashPasswordSHA256(newPassword);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Password successfully changed.";
                TempData["logOutMessage"] = null;
                TempData["ErrorMessage"] = null;
                return RedirectToAction("Login", "Home");
            }

            TempData["ErrorMessage"] = "An error occurred.";
            return RedirectToAction("ResetPassword", "Home", new { userId = userId });
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();  // Clear the session
            TempData["logOutMessage"] = "Logout successful.";
            TempData["SuccessMessage"] = null;
            TempData["ErrorMessage"] = null;
            return RedirectToAction("Login", "Home");  // Redirect to Login action in Home controller
        }

        public static string HashPasswordSHA256(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private bool VerifyPassword(string providedPassword, string storedHash)
        {
            string hashedPassword = HashPasswordSHA256(providedPassword);

            return hashedPassword.Equals(storedHash);
        }


    }
}
