using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace CollegeQuizWeb.Dto.Admin;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Utils;
using System.ComponentModel.DataAnnotations;

public class AddUserDto
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
    

    public string? Password { get; set; }
    

    public string? Email { get; set; }
    
    public bool IsAccountActivated { get; set; }
    
    public long? Id { get; set; }

}

public class AddUserDtoPayload : AbstractControllerPayloader<AdminController>
{
    public AddUserDto Dto { get; set; }
    
    public AddUserDtoPayload(AdminController controllerReference)
        : base(controllerReference) { }
}