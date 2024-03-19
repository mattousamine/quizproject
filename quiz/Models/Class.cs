using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quiz.Models
{
    [Table("multiplayer_quiz")]
    public class MultiPlayerQuiz
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [ForeignKey("Quiz")]
        [Column("quiz_id")]
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }

        [Column("begin_date")]
        public DateTime BeginDate { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }


        [Column("qr_link")]
        [MaxLength(255)] 
        public string QrLink { get; set; }

        [Column("site_link")]
        [MaxLength(255)] 
        public string SiteLink { get; set; }
    }
}
