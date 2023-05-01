namespace CollegeQuizWeb.Dto.User;

using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Utils;
using System.ComponentModel.DataAnnotations;

public class EditProfileDto
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
    
    public string? OldPassword { get; set; }
    
    public string? NewPassword { get; set; }
    
    public string? ConfirmPassword { get; set; }
    
}

public class EditProfileDtoPayload : AbstractControllerPayloader<UserController>
{
    public EditProfileDto Dto { get; set; }
    
    public EditProfileDtoPayload(UserController controllerReference)
        : base(controllerReference) { }
}