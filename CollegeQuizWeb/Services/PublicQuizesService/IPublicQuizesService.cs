using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Dto.PublicQuizes;
using CollegeQuizWeb.Dto.Quiz;
using CollegeQuizWeb.Entities;

namespace CollegeQuizWeb.Services.PublicQuizesService;

public interface IPublicQuizesService
{
    /// <summary>
    /// Method that get public quizes
    /// for Quizes view
    /// </summary>
    /// <returns>Public quizes</returns>
    Task<List<MyQuizDto>> GetPublicQuizes();
    
    /// <summary>
    /// Method that filter public quizes
    /// for Quizes view
    /// </summary>
    /// <param name="obj">PublicDtoPayLoad with public quiz data</param>
    /// <returns>Filtered public quizes</returns>
    Task<List<MyQuizDto>> FilterQuizes(PublicDtoPayLoad obj);
    
    /// <summary>
    /// Method that get public quiz information
    /// for Quizes view
    /// </summary>
    /// <param name="id">user id</param>
    /// <param name="controller">PublicQuizesController instance</param>
    Task PublicQuizInfo(long id, PublicQuizesController controller);
    
    /// <summary>
    /// Method that share public quiz
    /// </summary>
    /// <param name="id">user id</param>
    /// <param name="controller">PublicQuizesController instance</param>
    Task Share(string id, PublicQuizesController controller);
    
    /// <summary>
    /// Method that get caterories
    /// for Quizes view
    /// </summary>
    /// <param name="controller">PublicQuizesController instance</param>
    Task Categories(PublicQuizesController controller);
    
    /// <summary>
    /// Method that get filtered public quizes by category
    /// for Quizes view
    /// </summary>
    /// <param name="controller">PublicQuizesController instance</param>
    /// <param name="categories">quiz category</param>
    /// <returns>Filtered public quizes by category</returns>
    Task<List<SharedQuizesEntity>> Filter(PublicQuizesController controller, string[] categories);
}