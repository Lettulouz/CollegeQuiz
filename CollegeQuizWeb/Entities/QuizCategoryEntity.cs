using System.ComponentModel.DataAnnotations.Schema;
using CollegeQuizWeb.DbConfig;

namespace CollegeQuizWeb.Entities;

[Table("quiz_categories")]
public class QuizCategoryEntity
{
    [Column("quiz_id")]
    public long QuizId { get; set; }
    
    [Column("category_id")]
    public long CategoryId { get; set; }
    
    public virtual QuizEntity QuizEntity { get; set; }
    public virtual CategoryEntity CategoryEntity { get; set; }
}