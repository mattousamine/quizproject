using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quiz.Models
{
    [Table("session")]
    public class Session
    {
        [Key]
        [Column("session_id")]
        public int SessionDBId { get; set; }

        [Column("user_id")]
        public string UserId { get; set; }

        [Column("start_time")]
        public DateTime StartTime { get; set; }

        [Column("end_time")]
        public DateTime? EndTime { get; set; }
    }
}
