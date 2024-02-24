using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace quiz.Models
{
    [Table("question")]
    public class Question
    {
        [Key]
        [Column("question_id")]
        public int QuestionId { get; set; }

        [Required]
        [Column("question_text", TypeName = "text")]
        public string QuestionText { get; set; }

        [Column("question_feedback", TypeName = "text")]
        public string QuestionFeedback { get; set; }

        [Required]
        [Column("question_level")]
        public short QuestionLevel { get; set; }

        [Required]
        [Column("question_is_active")]
        public bool QuestionIsActive { get; set; }
    }
}
