using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CollegeQuizWeb.DbConfig;

namespace CollegeQuizWeb.Entities;

[Table("questions")]
public class QuestionEntity : AbstractAuditableEntity
{
    [Required]
    [Column("index")]
    public int Index { get; set; }
    
    [Required]
    [Column("name")]
    public string Name { get; set; }
    
    [Required]
    [Column("time_min")]
    public int TimeMin { get; set; }
    
    [Required]
    [Column("time_sec")]
    public int TimeSec { get; set; }
    
    [ForeignKey(nameof(QuizEntity))]
    [Column("quiz_id")]
    public long QuizId { get; set; }

    [ForeignKey(nameof(QuestionTypeEntity))]
    [Column("question_type_id")]
    public int QuestionType { get; set; }
    public virtual QuizEntity QuizEntity { get; set; }
    public virtual QuestionTypeEntity QuestionTypeEntity { get; set; }
}