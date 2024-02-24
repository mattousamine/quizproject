using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace quiz.Models
{
    [Table("option")]
    public class Option
    {
        [Key]
        [Column("option_id")]
        public int OptionId { get; set; }

        [Required]
        [Column("option_text", TypeName = "text")]
        public string OptionText { get; set; }

        [Required]
        [Column("option_is_correct")]
        public bool OptionIsCorrect { get; set; }
    }
}
