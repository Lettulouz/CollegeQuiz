using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CollegeQuizWeb.DbConfig;

namespace CollegeQuizWeb.Entities;

[Table("quiz_name")]
public class QuizEntity : AbstractAuditableEntity
{
    [Required]
    [Column("quiz_id")]
    public string Quiz_id { get; set; }
    
    [Required]
    [Column("quizName")]
    public string QuizName { get; set; }
    
    [Required]
    [Column("quiz_owner")]
    public string Quiz_owner { get; set; }
}