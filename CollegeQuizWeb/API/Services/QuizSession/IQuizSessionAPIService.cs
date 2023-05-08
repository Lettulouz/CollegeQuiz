using System.Threading.Tasks;
using CollegeQuizWeb.API.Dto;

namespace CollegeQuizWeb.API.Services.QuizSession;

public interface IQuizSessionAPIService
{
    /// <summary>
    /// Method that join user to quiz lobby
    /// </summary>
    /// <param name="loggedUsername">Current user</param>
    /// <param name="connectionId">Connection Id</param>
    /// <param name="token">Join code</param>
    /// <returns>JoinToSessionDto</returns>
    Task<JoinToSessionDto> JoinRoom(string loggedUsername, string connectionId, string token);
    
    /// <summary>
    /// Method that let user leave quiz lobby
    /// </summary>
    /// <param name="loggedUsername">Current user</param>
    /// <param name="connectionId">Connection Id</param>
    /// <param name="token">Join code</param>
    /// <returns>SimpleResponseDto</returns>
    Task<SimpleResponseDto> LeaveRoom(string loggedUsername, string connectionId, string token);
    
    /// <summary>
    /// Method that make host room
    /// </summary>
    /// <param name="loggedUsername">Current user</param>
    /// <param name="connectionId">Connection Id</param>
    /// <param name="token">Join code</param>
    /// <returns>JoinToSessionDto</returns>
    Task<JoinToSessionDto> EstabilishedHostRoom(string loggedUsername, string connectionId, string token);
    
    /// <summary>
    /// Method that get lobby data
    /// </summary>
    /// <param name="loggedUsername">Current user</param>
    /// <param name="token">Join code</param>
    /// <returns>QuizLobbyInfoDto</returns>
    Task<QuizLobbyInfoDto> GetLobbyData(string loggedUsername, string token);
    
    /// <summary>
    /// Method that send user's answers
    /// </summary>
    /// <param name="connectionId">Connection Id</param>////
    /// <param name="questionId">Question Id</param>///
    /// <param name="answerId">Answer Id</param>///
    /// <param name="isMultiAnswer">Is this question multi answer</param>///
    Task SendAnswer(string connectionId, string questionId, string answerId, bool isMultiAnswer);
    
    /// <summary>
    /// Method that remove user from session
    /// </summary>
    /// <param name="loggedUsername">Current user</param>
    /// <param name="token">Join code</param>
    /// <param name="username">User username</param>
    /// <returns>SimpleResponseDto</returns>
    Task<SimpleResponseDto> RemoveFromSession(string loggedUsername, string token, string username);
    
    /// <summary>
    /// Method that ban user from session
    /// </summary>
    /// <param name="loggedUsername">Current user</param>
    /// <param name="token">Join code</param>
    /// <param name="username">User username</param>
    /// <returns>SimpleResponseDto</returns>
    Task<SimpleResponseDto> BanFromSession(string loggedUsername, string token, string username);
    
    /// <summary>
    /// Method that unban user from session
    /// </summary>
    /// <param name="loggedUsername">Current user</param>
    /// <param name="token">Join code</param>
    /// <param name="username">User username</param>
    /// <returns>SimpleResponseDto</returns>
    Task<SimpleResponseDto> UnbanFromSession(string loggedUsername, string token, string username);
}