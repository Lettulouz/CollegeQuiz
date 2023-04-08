using System.Threading.Tasks;
using CollegeQuizWeb.API.Dto;

namespace CollegeQuizWeb.API.Services.Quiz;

public interface IQuizAPIService
{
    Task<SimpleResponseDto> AddQuizQuestions(long quizId, AggregateQuestionsReqDto dtos, string loggedUsername);
    Task<AggregateQuestionsReqDto> GetQuizQuestions(long quizId, string loggedUsername);
}