using System.ComponentModel.DataAnnotations;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Utils;

namespace CollegeQuizWeb.Dto.ChangePassword;

public class ChangePasswordDto
{
    [Required(ErrorMessage = Lang.PASSWORD_REQUIRED)]
    [RegularExpression(RegexApp.MIN_ONE_UPPER_LOWER_NUM_SPEC, ErrorMessage = Lang.PASSWORD_REGEX_ERROR)]
    public string NewPassword { get; set; }
    
    [Required(ErrorMessage = Lang.PASSWORD_REQUIRED)]
    public string RepeatNewPassword { get; set; }
}

public class ChangePasswordPayloadDto : AbstractControllerPayloader<AuthController>
{
    public ChangePasswordDto Dto { get; set; }
    
    public ChangePasswordPayloadDto(AuthController controllerReference)
        : base(controllerReference) { }
}