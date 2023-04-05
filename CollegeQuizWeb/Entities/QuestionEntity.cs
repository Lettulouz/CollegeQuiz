using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CollegeQuizWeb.DbConfig;

namespace CollegeQuizWeb.Entities;

[Table("questions")]
public class QuestionEntity : AbstractAuditableEntity
{
    [Required]
    [Column("quiz_id")]
    public string Quiz_id { get; set; }
    
    
    [Required]
    [Column("question")]
    public string Question { get; set; }
    
}