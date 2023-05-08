using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeQuizWeb.API.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollegeQuizWeb.API.Services.Quiz;

public interface IQuizAPIService
{
 
    /// <summary>
    /// Method that add questions to quiz
    /// </summary>
    /// <param name="quizId">quiz Id</param>
    /// <param name="dtos">Dto with questions</param>
    /// <param name="loggedUsername">user username</param>
    /// <returns>added questions</returns>
    Task<SimpleResponseDto> AddQuizQuestions(long quizId, AggregateQuestionReq2Dto dtos, string loggedUsername);
    
    /// <summary>
    /// Method tha get questions of quiz
    /// </summary>
    /// <param name="quizId">quiz Id</param>
    /// <param name="loggedUsername">User username</param>
    /// <param name="controller">QuizAPIController Instance</param>
    /// <returns>Dto with quiz questions</returns>
    Task<AggregateQuestionsReqDto> GetQuizQuestions(long quizId, string loggedUsername, Controller controller);
    
    /// <summary>
    /// Method that update quiz name
    /// </summary>
    /// <param name="quizId">quiz Id</param>
    /// <param name="newQuizName">New Quiz name</param>
    /// <param name="loggedUsername">user username</param>
    /// <returns>result</returns>
    Task<SimpleResponseDto> UpdateQuizName(long quizId, string newQuizName, string loggedUsername);
    
    /// <summary>
    /// Method that update quiz images
    /// </summary>
    /// <param name="quizId">quiz Id</param>
    /// <param name="uploads">Uploaded images</param>
    /// <param name="loggedUsername">user username</param>
    /// <param name="controller">QuizController Instance</param>
    /// <returns>list of images</returns>
    Task<QuizImagesResDto> UpdateQuizImages(long quizId, List<IFormFile?> uploads, string loggedUsername, Controller controller);
}