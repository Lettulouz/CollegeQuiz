using CollegeQuizWeb.Controllers;

namespace CollegeQuizWeb.Dto.PublicQuizes;

public class PublicQuizesDto
{
    public string Name { get; set; }
}

public class PublicDtoPayLoad : AbstractControllerPayloader<PublicQuizesController>
{
    public PublicQuizesDto Dto { get; set; }
    
    public PublicDtoPayLoad(PublicQuizesController controllerReference)
        : base(controllerReference){}
}