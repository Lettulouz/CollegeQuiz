using System.Threading.Tasks;
using CollegeQuizWeb.API.Dto;

namespace CollegeQuizWeb.API.Services.QuizSession;

public interface IQuizSessionAPIService
{
    Task<SimpleResponseDto> ValidateAndJoinRoom(string loggedUsername, string connectionId, string sessionId);
    Task<SimpleResponseDto> LeaveRoom(string loggedUsername, string connectionId, string sessionId);
}