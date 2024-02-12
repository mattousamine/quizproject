using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace quiz.Models
{
    [Table("category")]
    public class Category
    {
        [Key]
        [Column("category_id")]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("category_name")]
        public string CategoryName { get; set; }

        [Required]
        [Column("category_is_active")]
        public bool CategoryIsActive { get; set; }
    }
}
