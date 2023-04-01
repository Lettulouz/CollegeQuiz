using System.ComponentModel.DataAnnotations;
using CollegeQuizWeb.Utils;

namespace CollegeQuizWeb.Dto;

public class RegisterDto
{
    [Required(ErrorMessage = Lang.FIRST_NAME_IS_REQUIRED_ERROR)]
    [RegularExpression(RegexApp.ALL_LETTERS_EXCEPT_NUM_AND_SPEC, ErrorMessage = Lang.FirstNameRegexError)]
    [MinLength(2, ErrorMessage = Lang.FirstNameTooShortError)]
    [MaxLength(35, ErrorMessage = Lang.FirstNameTooLongError)]
    public string FirstName { get; set; }
    [Required(ErrorMessage = Lang.LAST_NAME_IS_REQUIRED_ERROR)]
    [RegularExpression(RegexApp.ALL_LETTERS_EXCEPT_NUM_AND_SPEC, ErrorMessage = Lang.LastNameRegexError)]
    [MinLength(2, ErrorMessage = Lang.LastNameTooShortError)]
    [MaxLength(35, ErrorMessage = Lang.LastNameTooLongError)]
    public string LastName { get; set; }
    [Required(ErrorMessage = Lang.USERNAME_IS_REQUIRED_ERROR)]
    [RegularExpression(RegexApp.ONLY_SMALL_LETTERS_NUM, ErrorMessage = Lang.UsernameRegexError)]
    [MinLength(5, ErrorMessage = Lang.UsernameTooShortError)]
    [MaxLength(25, ErrorMessage = Lang.UsernameTooLongError)]
    public string Username { get; set; }
    [Required(ErrorMessage = Lang.PASSWORD_IS_REQUIRED_ERROR)]
    [RegularExpression(RegexApp.MIN_ONE_UPPER_LOWER_NUM_SPEC, ErrorMessage = Lang.PasswordRegexError)]
    [MinLength(8, ErrorMessage = Lang.PasswordTooShortError)]
    [MaxLength(25, ErrorMessage = Lang.PasswordTooLongError)]
    public string Password { get; set; }
    [Required(ErrorMessage = Lang.EMAIL_IS_REQUIRED_ERROR)]
    [EmailAddress(ErrorMessage = Lang.EmailNotCorrectError)]
    [MaxLength(254, ErrorMessage = Lang.EmailTooLongError)]
    public string Email { get; set; }
    public int TeamID { get; set; }
    [Required]
    public bool RulesAccept { get; set; }
}