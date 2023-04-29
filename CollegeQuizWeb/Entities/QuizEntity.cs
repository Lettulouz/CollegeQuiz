using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CollegeQuizWeb.DbConfig;

namespace CollegeQuizWeb.Entities;

[Table("quizes")]
public class QuizEntity : AbstractAuditableEntity
{
    [Required]
    [Column("name")]
    public string Name { get; set; }
    
    [Required]
    [Column("is_public")]
    public bool IsPublic { get; set; }
    
    [ForeignKey(nameof(UserEntity))]
    [Column("user_id")]
    public long UserId { get; set; }
    
    public virtual UserEntity UserEntity { get; set; }
    public virtual ICollection<SharedQuizesEntity> SharedQuizesEntities { get; set; }
    
    public virtual ICollection<QuizCategoryEntity> QuizCategoryEntities { get; set; }
}