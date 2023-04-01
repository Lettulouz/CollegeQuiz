using System.ComponentModel.DataAnnotations;
using CollegeQuizWeb.Utils;

namespace CollegeQuizWeb.Dto;

public class RegisterDto
{
    [Required(ErrorMessage = Lang.FIRST_NAME_IS_REQUIRED_ERROR)]
    [RegularExpression(RegexApp.ALL_LETTERS_EXCEPT_NUM_AND_SPEC, ErrorMessage = Lang.FIRST_NAME_REGEX_ERROR)]
    [MinLength(2, ErrorMessage = Lang.FIRST_NAME_TOO_SHORT_ERROR)]
    [MaxLength(35, ErrorMessage = Lang.FIRST_NAME_TOO_LONG_ERROR)]
    public string FirstName { get; set; }
    
    [Required(ErrorMessage = Lang.LAST_NAME_IS_REQUIRED_ERROR)]
    [RegularExpression(RegexApp.ALL_LETTERS_EXCEPT_NUM_AND_SPEC, ErrorMessage = Lang.LAST_NAME_REGEX_ERROR)]
    [MinLength(2, ErrorMessage = Lang.LAST_NAME_TOO_SHORT_ERROR)]
    [MaxLength(35, ErrorMessage = Lang.LAST_NAME_TOO_LONG_ERROR)]
    public string LastName { get; set; }
    
    [Required(ErrorMessage = Lang.USERNAME_IS_REQUIRED_ERROR)]
    [RegularExpression(RegexApp.ONLY_SMALL_LETTERS_NUM, ErrorMessage = Lang.USERNAME_REGEX_ERROR)]
    [MinLength(5, ErrorMessage = Lang.USERNAME_TOO_SHORT_ERROR)]
    [MaxLength(25, ErrorMessage = Lang.USERNAME_TOO_LONG_ERROR)]
    public string Username { get; set; }
    
    [Required(ErrorMessage = Lang.PASSWORD_IS_REQUIRED_ERROR)]
    [RegularExpression(RegexApp.MIN_ONE_UPPER_LOWER_NUM_SPEC, ErrorMessage = Lang.PASSWORD_REGEX_ERROR)]
    [MinLength(8, ErrorMessage = Lang.PASSWORD_TO_SHORT_ERROR)]
    [MaxLength(25, ErrorMessage = Lang.PASSWORD_TO_LONG_ERROR)]
    public string Password { get; set; }
    
    [Required(ErrorMessage = Lang.EMAIL_IS_REQUIRED_ERROR)]
    [EmailAddress(ErrorMessage = Lang.EMAIL_INCORRECT_ERROR)]
    [MaxLength(254, ErrorMessage = Lang.EMAIL_TOO_LONG_ERROR)]
    public string Email { get; set; }
    
    public int TeamID { get; set; }
    
    [Required]
    public bool RulesAccept { get; set; }
}