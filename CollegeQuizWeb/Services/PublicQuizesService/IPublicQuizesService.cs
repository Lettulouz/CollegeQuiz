using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Dto.PublicQuizes;
using CollegeQuizWeb.Dto.Quiz;

namespace CollegeQuizWeb.Services.PublicQuizesService;

public interface IPublicQuizesService
{
    Task<List<MyQuizDto>> GetPublicQuizes();
    Task<List<MyQuizDto>> FilterQuizes(PublicDtoPayLoad obj);
    Task PublicQuizInfo(long id, PublicQuizesController controller);
    Task Share(string id, PublicQuizesController controller);
}