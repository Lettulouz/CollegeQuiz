using System;
using System.ComponentModel;
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
    
    [Column("account_status")]
    [DefaultValue(1)]
    [Range(1,3)]
    public int AccountStatus { get; set; }
    
    [Column("current_status_expiration_date")]
    public DateTime CurrentStatusExpirationDate { get; set; }
    
    [Column("team_id")]
    public int TeamID { get; set; }
    
    [Required]
    [Column("rules_accept")]
    public bool RulesAccept { get; set; }
    
    [DefaultValue(0)]
    [Column("is_account_activated")]
    public bool IsAccountActivated { get; set; }
}