using System.ComponentModel.DataAnnotations;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Utils;

namespace CollegeQuizWeb.Dto;

public class LoginDto
{
    [Required(ErrorMessage = Lang.USERNAME_OR_EMAIL_REQUIRED)]
    [RegularExpression(RegexApp.ALL_LETTERS_AND_NUM_EXC_SPECIAL, ErrorMessage = Lang.USERNAME_OR_EMAIL_INVALID_CHARS)]
    public string LoginOrEmail { get; set; }
    
    [Required(ErrorMessage = Lang.PASSWORD_IS_REQUIRED_ERROR)]
    [RegularExpression(RegexApp.MIN_ONE_UPPER_LOWER_NUM_SPEC, ErrorMessage = Lang.PASSWORD_REGEX_ERROR)]
    [MinLength(8, ErrorMessage = Lang.PASSWORD_TO_SHORT_ERROR)]
    [MaxLength(25, ErrorMessage = Lang.PASSWORD_TO_LONG_ERROR)]
    public string Password { get; set; }
}

public class LoginDtoPayload : AbstractControllerPayloader<AuthController>
{
    public LoginDto Dto { get; set; }
    
    public LoginDtoPayload(AuthController controllerReference)
        : base(controllerReference) { }
}