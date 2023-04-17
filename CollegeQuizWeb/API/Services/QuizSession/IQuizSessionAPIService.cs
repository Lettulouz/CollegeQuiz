using System.Threading.Tasks;
using CollegeQuizWeb.API.Dto;

namespace CollegeQuizWeb.API.Services.QuizSession;

public interface IQuizSessionAPIService
{
    Task<JoinToSessionDto> JoinRoom(string loggedUsername, string connectionId, string token);
    Task<SimpleResponseDto> LeaveRoom(string loggedUsername, string connectionId, string token);
    Task<JoinToSessionDto> EstabilishedHostRoom(string loggedUsername, string connectionId, string token);
    Task<QuizLobbyInfoDto> GetLobbyData(string loggedUsername, string token);
    Task SendAnswer(string connectionId, string token, string questionId, string answerId);
}