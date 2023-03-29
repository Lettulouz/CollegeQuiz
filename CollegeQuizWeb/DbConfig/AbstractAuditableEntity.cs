using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeQuizWeb.DbConfig;

public abstract class AbstractAuditableEntity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }
 
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
        
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}