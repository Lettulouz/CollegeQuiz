using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CollegeQuizWeb.DbConfig;

namespace CollegeQuizWeb.Entities;

[Table("users_questions_answers")]
public class UsersQuestionsAnswersEntity : AbstractAuditableEntity
{
    [Required]
    [ForeignKey(nameof(QuizSessionParticEntity))]
    [Column("id_of_connections")]
    public long ConnectionId { get; set; }
    
    [Required]
    [Column("question")]
    public int Question { get; set; }
    
    [Column("answer")]
    public int Answer { get; set; }
    
    [Column("range_answer")]
    public string? Range { get; set; }
    public virtual QuizSessionParticEntity QuizSessionParticEntity { get; set; }
}