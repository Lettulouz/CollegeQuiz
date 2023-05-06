using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Dto.SharedQuizes;

namespace CollegeQuizWeb.Services.SharedQuizesService;

public interface ISharedQuizesService
{
    /// <summary>
    /// Method that shere quiz token with other users
    /// </summary>
    /// <param name="obj">Dto with sharetoken data</param>
    Task ShareQuizToken(ShareTokenPayloadDto obj);
    
    /// <summary>
    /// Method that share quiz information with other users
    /// </summary>
    /// <param name="id">Quiz id</param>
    /// <param name="controller">SharedQuizesController instance</param>
    Task ShareQuizInfo(long id, SharedQuizesController controller);
}