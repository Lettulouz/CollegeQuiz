using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeQuizWeb.API.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollegeQuizWeb.API.Services.Quiz;

public interface IQuizAPIService
{
    Task<SimpleResponseDto> AddQuizQuestions(long quizId, AggregateQuestionReq2Dto dtos, string loggedUsername);
    Task<AggregateQuestionsReqDto> GetQuizQuestions(long quizId, string loggedUsername, Controller controller);
    Task<SimpleResponseDto> UpdateQuizName(long quizId, string newQuizName, string loggedUsername);
    Task<QuizImagesResDto> UpdateQuizImages(long quizId, List<IFormFile?> uploads, string loggedUsername, Controller controller);
}