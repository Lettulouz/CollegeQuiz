using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeQuizWeb.Entities;

[Table("shared_quizes")]
public class SharedQuizes
{
    [Required]
    [Column("quiz_id")]
    [ForeignKey(nameof(QuizEntity))]
    public long QuizId { get; set; }
    public virtual QuizEntity QuizEntity { get; set; }
    
    [Required]
    [Column("user_id")]
    [ForeignKey(nameof(UserEntity))]
    public long UserId { get; set; }
    public virtual UserEntity UserEntity { get; set; }
}