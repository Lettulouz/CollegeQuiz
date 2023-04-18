using System.Threading.Tasks;
using CollegeQuizWeb.Dto.SharedQuizes;

namespace CollegeQuizWeb.Services.SharedQuizesService;

public interface ISharedQuizesService
{
    Task ShareQuizToken(ShareTokenPayloadDto obj);
}