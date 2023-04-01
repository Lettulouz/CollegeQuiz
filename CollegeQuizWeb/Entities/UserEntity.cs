using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CollegeQuizWeb.DbConfig;

namespace CollegeQuizWeb.Entities;

[Table("users")]
public class UserEntity : AbstractAuditableEntity
{
    [Required]
    [Column("first_name")]
    public string FirstName { get; set; }
    
    [Required]
    [Column("last_name")]
    public string LastName { get; set; }
    
    [Required]
    [Column("username")]
    public string Username { get; set; }
    
    [Required]
    [Column("email")]
    public string Email { get; set; }
    
    [Required]
    [Column("password")]
    public string Password { get; set; }
    
    [Column("team_id")]
    public int TeamID { get; set; }
    
    [Required]
    [Column("rules_accept")]
    public bool RulesAccept { get; set; }
}