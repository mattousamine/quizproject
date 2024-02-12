using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace quiz.Models
{
    [Table("question_option")]
    public class QuestionOption
    {
        [Key]
        [Column("question_option_id")]
        public int QuestionOptionId { get; set; }

        [ForeignKey("Question")]
        [Column("question_id")]
        public int QuestionId { get; set; }
        public Question Question { get; set; }

        [ForeignKey("Option")]
        [Column("option_id")]
        public int OptionId { get; set; }
        public Option Option { get; set; }
    }
}
