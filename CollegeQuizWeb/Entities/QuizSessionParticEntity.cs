using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CollegeQuizWeb.DbConfig;

namespace CollegeQuizWeb.Entities;

[Table("quiz_session_participants")]
public class QuizSessionParticEntity : AbstractAuditableEntity
{
    [Required]
    [Column("connection_id")]
    public string ConnectionId { get; set; }
    
    [Required]
    [Column("score")]
    public long Score { get; set; }
    
    [Required]
    [Column("current_streak")]
    public int CurrentStreak { get; set; }
    
    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; }
    
    [Required]
    [Column("is_banned")]
    public bool IsBanned { get; set; }
    
    [ForeignKey(nameof(QuizLobbyEntity))]
    [Column("quiz_lobby_id")]
    public long QuizLobbyId { get; set; }
    
    [ForeignKey(nameof(UserEntity))]
    [Column("participant_id")]
    public long ParticipantId { get; set; }
    
    public virtual UserEntity UserEntity { get; set; }
    public virtual QuizLobbyEntity QuizLobbyEntity { get; set; }
}