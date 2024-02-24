using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace quiz.Models
{
    [Table("quiz_tracker")]
    public class QuizTracker
    {
        [Key]
        [Column("quiz_tracker_id")]
        public int QuizTrackerId { get; set; }

        [ForeignKey("UserQuiz")]
        [Column("user_quiz_id")]
        public int UserQuizId { get; set; }
        public UserQuiz UserQuiz { get; set; }

        [ForeignKey("QuizSession")]
        [Column("quiz_session_id")]
        public int QuizSessionId { get; set; }
        public QuizSession QuizSession { get; set; }

        [ForeignKey("Quiz")]
        [Column("quiz_id")]
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }
    }
}
