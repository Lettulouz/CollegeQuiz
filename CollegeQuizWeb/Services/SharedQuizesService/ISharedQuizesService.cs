using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Dto.SharedQuizes;

namespace CollegeQuizWeb.Services.SharedQuizesService;

public interface ISharedQuizesService
{
    /// <summary>
    /// Method that is being used to shere quiz token with other users
    /// </summary>
    /// <param name="obj">Dto with sharetoken data</param>
    Task ShareQuizToken(ShareTokenPayloadDto obj);
    
    /// <summary>
    /// Method that is being used to share quiz information with other users
    /// </summary>
    /// <param name="id">Quiz id</param>
    /// <param name="controller">Pass data into controller</param>
    Task ShareQuizInfo(long id, SharedQuizesController controller);
}