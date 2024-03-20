using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quiz.Models
{
    [Table("multiplayer_tmp_session")]
    public class MultiplayerTmpSession
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("username")]
        [MaxLength(191)]
        public string Username { get; set; }

        [ForeignKey("Quiz")]
        [Column("quiz_id")]
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }

        [Column("score")]
        public int Score { get; set; }
    }
}
