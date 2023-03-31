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
    [RegularExpression(RegexApp.ALL_LETTERS_EXCEPT_NUM_AND_SPEC, ErrorMessage = Lang.FirstNameRegexError)]
    [MinLength(2, ErrorMessage = Lang.FirstNameTooShortErrorError)]
    [MaxLength(35, ErrorMessage = Lang.FirstNameTooLongErrorError)]
    [Column("first_name")]
    public string FirstName { get; set; }
    [Required]
    [RegularExpression(RegexApp.ALL_LETTERS_EXCEPT_NUM_AND_SPEC, ErrorMessage = Lang.LastNameRegexError)]
    [MinLength(2, ErrorMessage = Lang.LastNameTooShortErrorError)]
    [MaxLength(35, ErrorMessage = Lang.LastNameTooLongErrorError)]
    [Column("last_name")]
    public string LastName { get; set; }
    [Required]
    [RegularExpression(RegexApp.ONLY_SMALL_LETTERS_NUM, ErrorMessage = Lang.UsernameRegexError)]
    [MinLength(5, ErrorMessage = Lang.UsernameTooShortError)]
    [MaxLength(25, ErrorMessage = Lang.UsernameTooLongError)]
    [Column("username")]
    public string Username { get; set; }
    [Required]
    [EmailAddress(ErrorMessage = Lang.EmailNotCorrectError)]
    [MaxLength(254, ErrorMessage = Lang.EmailTooLongError)]
    [Column("email")]
    public string Email { get; set; }
    [Required]
    [RegularExpression(RegexApp.MIN_ONE_UPPER_LOWER_NUM_SPEC, ErrorMessage = Lang.PasswordRegexError)]
    [MinLength(8, ErrorMessage = Lang.PasswordTooShortError)]
    [MaxLength(25, ErrorMessage = Lang.PasswordTooLongError)]
    [Column("password")]
    public string Password { get; set; }
    [Column("team_id")]
    public int TeamID { get; set; }
    [Column("rules_accept")]
    public bool RulesAccept { get; set; }
}