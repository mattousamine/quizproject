using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace quiz.Models
{
    [Table("user_issue")]
    public class UserIssue
    {
        [Key]
        [Column("user_issue_id")]
        public int UserIssueId { get; set; }

        [ForeignKey("QuizIssue")]
        [Column("quiz_issue_id")]
        public int QuizIssueId { get; set; }
        public QuizIssue QuizIssue { get; set; }

        [ForeignKey("UserQuiz")]
        [Column("user_quiz_id")]
        public int? UserQuizId { get; set; } // Nullable
        public UserQuiz UserQuiz { get; set; }
    }
}
