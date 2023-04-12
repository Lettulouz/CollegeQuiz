using System.Threading.Tasks;
using CollegeQuizWeb.API.Dto;

namespace CollegeQuizWeb.API.Services.QuizSession;

public interface IQuizSessionAPIService
{
    Task<JoinToSessionDto> JoinRoom(string loggedUsername, string connectionId, string token);
    Task<SimpleResponseDto> LeaveRoom(string loggedUsername, string connectionId, string token);
    
    // tylko do testowania
    Task SendMessage(string loggedUsername, string token, string message);
}