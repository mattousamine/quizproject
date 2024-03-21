using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quiz.Models
{
    [Table("MultiUserSession")]
    public class MultiUserSession
    {
        [Key]
        [Column("session_id")]
        public int SessionDBId { get; set; }

        [Column("user_id")]
        public string UserId { get; set; }

        [Column("username")]
        public string Username { get; set; }

        [Column("start_time")]
        public DateTime StartTime { get; set; }

        [Column("end_time")]
        public DateTime? EndTime { get; set; }
        public object SessionId { get; internal set; }
        public int QuizId { get; internal set; }
    }
}
