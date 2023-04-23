using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeQuizWeb.Dto.PublicQuizes;
using CollegeQuizWeb.Dto.Quiz;

namespace CollegeQuizWeb.Services.PublicQuizesService;

public interface IPublicQuizesService
{
    public Task<List<MyQuizDto>> GetPublicQuizes();
    public Task<List<MyQuizDto>> FilterQuizes(PublicDtoPayLoad obj);
}