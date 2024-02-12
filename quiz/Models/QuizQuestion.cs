using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace quiz.Models
{
    [Table("quiz_question")]
    public class QuizQuestion
    {
        [Key]
        [Column("quiz_question_id")]
        public int QuizQuestionId { get; set; }

        [ForeignKey("Quiz")]
        [Column("quiz_id")]
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }

        [ForeignKey("Question")]
        [Column("question_id")]
        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }
}
