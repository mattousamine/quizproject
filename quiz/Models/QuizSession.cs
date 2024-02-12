using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace quiz.Models
{
    [Table("quiz_session")]
    public class QuizSession
    {
        [Key]
        [Column("quiz_session_id")]
        public int QuizSessionId { get; set; }

        [Required]
        [Column("quiz_session_score")]
        public int QuizSessionScore { get; set; }

        [Column("quiz_session_time")]
        public DateTime QuizSessionTime { get; set; }
    }
}
