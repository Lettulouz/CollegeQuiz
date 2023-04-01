using System.ComponentModel.DataAnnotations;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Utils;

namespace CollegeQuizWeb.Dto.ChangePassword;

public class AttemptChangePasswordDto
{
    [Required(ErrorMessage = Lang.USERNAME_OR_EMAIL_REQUIRED)]
    [RegularExpression(RegexApp.ALL_LETTERS_AND_NUM_EXC_SPECIAL, ErrorMessage = Lang.USERNAME_OR_EMAIL_INVALID_CHARS)]
    public string LoginOrEmail { get; set; }
}

public class AttemptChangePasswordPayloadDto : AbstractControllerPayloader<AuthController>
{
    public AttemptChangePasswordDto Dto { get; set; }
    
    public AttemptChangePasswordPayloadDto(AuthController controllerReference) 
        : base(controllerReference) { }
}