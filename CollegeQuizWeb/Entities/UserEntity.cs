using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Utils;

namespace CollegeQuizWeb.Entities;

[Table("users")]
public class UserEntity : AbstractAuditableEntity
{
    [Required]
    [DisplayName("Imię")]
    [RegularExpression(RegexApp.ALL_LETTERS_EXCEPT_NUM_AND_SPEC, ErrorMessage = Lang.FIRST_NAME_REGEX_ERROR)]
    [MinLength(2, ErrorMessage = Lang.FIRST_NAME_TOO_SHORT_ERROR)]
    [MaxLength(35, ErrorMessage = Lang.FIRST_NAME_TOO_LONG_ERROR)]
    [Column("first_name")]
    public string FirstName { get; set; }
    
    [Required]
    [RegularExpression(RegexApp.ALL_LETTERS_EXCEPT_NUM_AND_SPEC, ErrorMessage = Lang.LAST_NAME_REGEX_ERROR)]
    [MinLength(2, ErrorMessage = Lang.LAST_NAME_TOO_SHORT_ERROR)]
    [MaxLength(35, ErrorMessage = Lang.LAST_NAME_TOO_LONG_ERROR)]
    [Column("last_name")]
    public string LastName { get; set; }
    
    [Required]
    [RegularExpression(RegexApp.ONLY_SMALL_LETTERS_NUM, ErrorMessage = Lang.USERNAME_REGEX_ERROR)]
    [MinLength(5, ErrorMessage = Lang.USERNAME_TOO_SHORT_ERROR)]
    [MaxLength(25, ErrorMessage = Lang.USERNAME_TOO_LONG_ERROR)]
    [Column("username")]
    public string Username { get; set; }
    
    [Required]
    [EmailAddress(ErrorMessage = Lang.EMAIL_INCORRECT_ERROR)]
    [MaxLength(254, ErrorMessage = Lang.EMAIL_TOO_LONG_ERROR)]
    [Column("email")]
    public string Email { get; set; }
    
    [Required]
    [RegularExpression(RegexApp.MIN_ONE_UPPER_LOWER_NUM_SPEC, ErrorMessage = Lang.PASSWORD_REGEX_ERROR)]
    [MinLength(8, ErrorMessage = Lang.PASSWORD_TO_SHORT_ERROR)]
    [Column("password")]
    public string Password { get; set; }
    
    [Column("team_id")]
    public int TeamID { get; set; }
    
    [Column("rules_accept")]
    public bool RulesAccept { get; set; }
}