using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Dto.Quiz;

namespace CollegeQuizWeb.Services.QuizService;

public interface IQuizService
{
    Task CreateNewQuiz(string loggedUsername, AddQuizDtoPayloader dtoPayloader);
    Task<List<MyQuizDto>> GetMyQuizes(string userLogin);
    Task<QuizDetailsDto> GetQuizDetails(string userLogin, long quizId, QuizController controller);
}