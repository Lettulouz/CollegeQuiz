using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CollegeQuizWeb.DbConfig;

namespace CollegeQuizWeb.Entities;

[Table("answers")]
public class AnswerEntity : AbstractAuditableEntity
{
    [Required]
    [Column("name")]
    public string Name { get; set; }
    
    [Required]
    [Column("is_good")]
    public bool IsGood { get; set; }
    
    [ForeignKey(nameof(QuestionEntity))]
    [Column("question_id")]
    public long QuestionId { get; set; }
    
    public virtual QuestionEntity QuestionEntity { get; set; }
}