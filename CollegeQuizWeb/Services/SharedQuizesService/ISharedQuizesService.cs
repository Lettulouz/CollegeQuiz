using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Dto.SharedQuizes;

namespace CollegeQuizWeb.Services.SharedQuizesService;

public interface ISharedQuizesService
{
    Task ShareQuizToken(ShareTokenPayloadDto obj);
    Task ShareQuizInfo(long id, SharedQuizesController controller);
}