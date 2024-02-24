using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quiz.Models
{
    [Table("category_quiz")]
    public class CategoryQuiz
    {
        [Key]
        [Column("category_quiz_id")]
        public int CategoryQuizId { get; set; }

        [ForeignKey("Category")]
        [Column("category_id")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        [ForeignKey("Quiz")]
        [Column("quiz_id")]
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }
    }
}
