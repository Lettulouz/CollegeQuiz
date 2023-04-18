using System.ComponentModel.DataAnnotations;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Utils;

namespace CollegeQuizWeb.Dto.SharedQuizes;

public class ShareTokenDto
{
        [Required(ErrorMessage = Lang.QUIZ_SHARED_TOKEN_ERROR)]
        public string ShareToken { get; set; }
        
}

public class ShareTokenPayloadDto : AbstractControllerPayloader<SharedQuizesController>
{
        public ShareTokenDto Dto { get; set; }
    
        public ShareTokenPayloadDto(SharedQuizesController controllerReference) 
                : base(controllerReference) { }
}