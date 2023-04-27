using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Dto.Quiz;
using CollegeQuizWeb.Dto.SharedQuizes;

namespace CollegeQuizWeb.Services.QuizService;

public interface IQuizService
{
    Task CreateNewQuiz(string loggedUsername, AddQuizDtoPayloader dtoPayloader);
    Task<List<MyQuizDto>> GetMyQuizes(string userLogin);
    Task<List<MyQuizSharedDto>> GetMyShareQuizes(string userLogin);
    Task<QuizDetailsDto> GetQuizDetails(string userLogin, long quizId, QuizController controller);
    Task<bool> CreateQuizCode(QuizController controller, string loggedUsername, long quizId);
    Bitmap GenerateQRCode(QuizController controller, string code);
}