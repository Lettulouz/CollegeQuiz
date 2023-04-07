using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CollegeQuizWeb.DbConfig;

namespace CollegeQuizWeb.Entities;

[Table("questions")]
public class QuestionEntity : AbstractAuditableEntity
{
    [Required]
    [Column("name")]
    public string Name { get; set; }
    
    [ForeignKey(nameof(QuizEntity))]
    [Column("quiz_id")]
    public long QuizId { get; set; }
    
    public virtual QuizEntity QuizEntity { get; set; }
}