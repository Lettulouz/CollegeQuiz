using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CollegeQuizWeb.DbConfig;

namespace CollegeQuizWeb.Entities;

[Table("category")]
public class CategoryEntity : AbstractAuditableEntity
{
    [Required]
    [Column("id")]
    public long CategoryId { get; set; }
    
    [Required]
    [Column("name")]
    public string Category { get; set; }

    public virtual ICollection<QuizCategoryEntity> QuizCategoryEntities { get; set; }
}