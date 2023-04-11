using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CollegeQuizWeb.DbConfig;

namespace CollegeQuizWeb.Entities;

[Table("quiz_lobby")]
public class QuizLobbyEntity : AbstractAuditableEntity
{
    [Required]
    [StringLength(5)]
    [Column("code")]
    public string Code { get; set; }
    
    [Required]
    [Column("is_expired")]
    public bool IsExpired { get; set; }
    
    [ForeignKey(nameof(UserEntity))]
    [Column("user_host_id")]
    public long UserHostId { get; set; }
    
    [ForeignKey(nameof(QuizEntity))]
    [Column("quiz_id")]
    public long QuizId { get; set; }
    
    public virtual UserEntity UserEntity { get; set; }
    public virtual QuizEntity QuizEntity { get; set; }
}