using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeQuizWeb.Entities;

[Table("shared_quizes")]
public class SharedQuizesEntity
{
    [Column("quiz_id")]
    public long QuizId { get; set; }
    
    [Column("user_id")]
    public long UserId { get; set; }
    
    public virtual QuizEntity QuizEntity { get; set; }
    public virtual UserEntity UserEntity { get; set; }
}