using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CollegeQuizWeb.DbConfig;

namespace CollegeQuizWeb.Entities;

[Table("quiz_tokens")]
public class ShareTokensEntity : AbstractAuditableEntity
{
    [Required]
    [StringLength(12)]
    [Column("token")]
    public string Token { get; set; }

    [ForeignKey(nameof(QuizEntity))]
    [Column("quiz_id")]
    public long QuizId { get; set; }
    
    public virtual QuizEntity QuizEntity { get; set; }
}