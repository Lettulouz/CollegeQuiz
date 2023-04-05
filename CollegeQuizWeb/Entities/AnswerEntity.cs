using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CollegeQuizWeb.DbConfig;

namespace CollegeQuizWeb.Entities;

[Table("answers")]
public class AnswerEntity : AbstractAuditableEntity
{
    [Required]
    [Column("quiz_id")]
    public string quiz_id { get; set; }
    
    [Required]
    [Column("answer")]
    public string Option { get; set; }
    
    [Required]
    [Column("isTrue")]
    public bool Is_true { get; set; }
    
}