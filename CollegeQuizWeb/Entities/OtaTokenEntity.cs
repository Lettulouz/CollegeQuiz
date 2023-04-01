using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CollegeQuizWeb.DbConfig;

namespace CollegeQuizWeb.Entities;

[Table("ota_tokens")]
public class OtaTokenEntity : AbstractAuditableEntity
{
    [Required]
    [StringLength(10)]
    [Column("token")]
    public string Token { get; set; }
    
    [Column("is_used")]
    public bool IsUsed { get; set; }
    
    [Required]
    [Column("expired_at")]
    public DateTime ExpiredAt { get; set; }
    
    [ForeignKey(nameof(UserEntity))]
    [Column("user_id")]
    public long UserId { get; set; }
    
    public virtual UserEntity UserEntity { get; set; }
}