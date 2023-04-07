using System.ComponentModel.DataAnnotations;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Utils;

namespace CollegeQuizWeb.Dto.Quiz;

public class AddQuizDto
{
    [Required(ErrorMessage = Lang.QUIZ_NAME_REQUIRED_ERROR)]
    [MinLength(2, ErrorMessage = Lang.QUIZ_NAME_TOO_SHORT_ERROR)]
    [MaxLength(50, ErrorMessage = Lang.QUIZ_NAME_TOO_LONG_ERROR)]
    public string QuizName { get; set; }
    
    [Required]
    public bool IsPrivate { get; set; }
}

public class AddQuizDtoPayloader : AbstractControllerPayloader<QuizController>
{
    public AddQuizDto Dto { get; set; }
    
    public AddQuizDtoPayloader(QuizController controllerReference)
        : base(controllerReference) { }
}