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
    
    [Required]
    [Column("is_range")]
    public bool IsRange { get; set; }
    
    [Column("step")]
    public int Step { get; set; }
    
    [Column("min_counted")]
    public int MinCounted { get; set; }
    
    [Column("max_counted")]
    public int MaxCounted { get; set; }
    
    [Column("min")]
    public int Min { get; set; }
    
    [Column("max")]
    public int Max { get; set; }
    public virtual QuestionEntity QuestionEntity { get; set; }
}