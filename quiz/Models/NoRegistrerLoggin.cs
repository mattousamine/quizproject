using System.ComponentModel.DataAnnotations;

namespace quiz.Models
{
    public class NoRegistrerLoggin
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 20 characters long.")]
        public string Username { get; set; }
    }
}
