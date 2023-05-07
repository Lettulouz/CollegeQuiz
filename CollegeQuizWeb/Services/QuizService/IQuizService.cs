using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Dto.Quiz;
using CollegeQuizWeb.Dto.SharedQuizes;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;

namespace CollegeQuizWeb.Services.QuizService;

public interface IQuizService
{
    /// <summary>
    /// Method that is being used to create new quiz for user
    /// </summary>
    /// <param name="loggedUsername">Current user Username</param>
    /// <param name="dtoPayloader">Dto with addquiz data</param>
    Task CreateNewQuiz(string loggedUsername, AddQuizDtoPayloader dtoPayloader);
    
    /// <summary>
    /// Method that get quizes from logged user
    /// </summary>
    /// <param name="userLogin">Current user Username</param>
    /// <returns>Logged user's quizes</returns>
    Task<List<MyQuizDto>> GetMyQuizes(string userLogin);
    
    /// <summary>
    /// Method that get shared quizes from logged user
    /// </summary>
    /// <param name="userLogin">Current user Username</param>
    /// <returns>Logged user's shared quizes</returns>
    Task<List<MyQuizSharedDto>> GetMyShareQuizes(string userLogin);
    
    /// <summary>
    /// Method that get quiz details
    /// </summary>
    /// <param name="userLogin">Current user Username</param>
    /// <param name="quizId">Quiz id</param>
    /// <param name="controller">QuizController instance</param>
    /// <returns>Quiz details</returns>
    Task<QuizDetailsDto> GetQuizDetails(string userLogin, long quizId, QuizController controller);
    
    /// <summary>
    /// Method that create quiz join code
    /// </summary>
    /// <param name="controller">QuizController instance</param>
    /// <param name="loggedUsername">Current user Username</param>
    /// <param name="quizId">Quiz id</param>
    /// <returns>True or false</returns>
    Task<bool> CreateQuizCode(QuizController controller, string loggedUsername, long quizId);
    
    
    /// <summary>
    /// Method that create quiz QR code the same as quiz join code
    /// </summary>
    /// <param name="controller">QuizController instance</param>
    /// <param name="code">Join code</param>
    /// <returns>QR code</returns>
    Task<SKBitmap> GenerateQRCode(QuizController controller, string code);
    
    /// <summary>
    /// Method that delete user's quiz
    /// </summary>
    /// <param name="quizId">Quiz id</param>
    /// <param name="loggedUsername">Current user Username</param>
    /// <param name="controller">Controller instance</param>
    Task DeleteQuiz(long quizId, string loggedUsername, Controller controller);
    
    /// <summary>
    /// Method that delete user's shared quiz
    /// </summary>
    /// <param name="quizId">Quiz id</param>
    /// <param name="loggedUsername">Current user Username</param>
    /// <param name="controller">Controller instance</param>
    Task DeleteSharedQuiz(long quizId, string loggedUsername, Controller controller);
}