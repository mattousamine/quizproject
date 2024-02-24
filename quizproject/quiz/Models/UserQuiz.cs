using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quiz.Models
{
    [Table("user_quiz")]
    public class UserQuiz
    {
        [Key]
        [Column("user_quiz_id")]
        public int UserQuizId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("user_quiz_username")]
        public string UserQuizUsername { get; set; }

        [Required]
        [StringLength(64)]
        [Column("user_quiz_password")]
        public string UserQuizPassword { get; set; }

        [Required]
        [StringLength(255)]
        [Column("user_quiz_email")]
        public string UserQuizEmail { get; set; }

        [Required]
        [Column("user_quiz_is_admin")]
        public bool UserQuizIsAdmin { get; set; }

        [Required]
        [Column("user_quiz_is_active")]
        public bool UserQuizIsActive { get; set; }
    }
}
