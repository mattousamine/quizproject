using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace quiz.Models
{
    [Table("quiz_issue")]
    public class QuizIssue
    {
        [Key]
        [Column("quiz_issue_id")]
        public int QuizIssueId { get; set; }

        [Required]
        [Column("quiz_issue_description", TypeName = "text")]
        public string QuizIssueDescription { get; set; }

        [Column("quiz_issue_date_reported")]
        public DateTime QuizIssueDateReported { get; set; }

        [Required]
        [Column("quiz_issue_is_fixed")]
        public bool QuizIssueIsFixed { get; set; }
    }
}
