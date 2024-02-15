using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quiz.Models
{
    [Table("quiz_img")]
    public class QuizImg
    {
        [Key]
        [Column("img_id")]
        public int ImgId { get; set; }

        [Required]
        [Column("quiz_id")]
        public int QuizId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("img")]
        public string Img { get; set; }

        [ForeignKey("QuizId")]
        public virtual Quiz Quiz { get; set; }
    }
}
