using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace quiz.Models
{
    [Table("quiz")]
    public class Quiz
    {
        [Key]
        [Column("quiz_id")]
        public int QuizId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("quiz_name")]
        public string QuizName { get; set; }

        [Required]
        [Column("quiz_is_active")]
        public bool QuizIsActive { get; set; }
    }
}
