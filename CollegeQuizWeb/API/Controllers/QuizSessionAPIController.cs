using System.Threading.Tasks;
using CollegeQuizWeb.API.Services.QuizSession;
using CollegeQuizWeb.Jwt;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollegeQuizWeb.API.Controllers;

[Route("/api/v1/dotnet/[controller]")]
public class QuizSessionAPIController : AbstractAPIController
{
    private readonly IQuizSessionAPIService _service;
    
    /// <summary>
    /// Controller tha handle game session
    /// </summary>
    /// <param name="service">QuizSessionAPIService</param>
    /// <param name="jwtService">jwtService</param>
    public QuizSessionAPIController(IQuizSessionAPIService service, IJwtService jwtService) : base(jwtService)
    {
        _service = service;
    }

    /// <summary>
    ///  Method that join user to quiz lobby
    /// </summary>
    /// <param name="connectionId">Connection Id </param>
    /// <param name="token">Join token</param>
    /// <returns>Json with join result, 403 status if user is not logged</returns>
    [HttpPost("[action]/{connectionId}/{token}")]
    public async Task<IActionResult> JoinRoom(string connectionId, string token)
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return StatusCode(403);
        
        return Json(await _service.JoinRoom(loggedUsername, connectionId, token));
    }

    /// <summary>
    /// Method that validate user token
    /// </summary>
    /// <param name="connectionId"></param>
    /// <param name="token">Join token</param>
    /// <returns>Json with join result, 403 status if user is not logged</returns>
    [HttpPost("[action]/{connectionId}/{token}")]
    public async Task<IActionResult> JoinRoomJwt(string connectionId, string token)
    {
        var user = await _jwtService.ValidateToken(this);
        if (user == null) return StatusCode(403);
        
        return Json(await _service.JoinRoom(user.Username, connectionId, token));
    }
    
    /// <summary>
    /// Method that let user to leave game room
    /// </summary>
    /// <param name="connectionId">connection id</param>
    /// <param name="token">join token</param>
    /// <returns>Leaving result</returns>
    [HttpPost("[action]/{connectionId}/{token}")]
    public async Task<IActionResult> LeaveRoom(string connectionId, string token)
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return StatusCode(403);
        
        return Json(await _service.LeaveRoom(loggedUsername, connectionId, token));
    }
    
    /// <summary>
    /// Method that allow to remove user from game session.
    /// </summary>
    /// <param name="token">Join token</param>
    /// <param name="username">user username</param>
    /// <returns>Status code</returns>
    [HttpPost("[action]/{token}/{username}")]
    public async Task<IActionResult> RemoveFromSession(string token, string username)
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return StatusCode(403);
        
        await _service.RemoveFromSession(loggedUsername, token, username);
        return StatusCode(200);
    }
    
    /// <summary>
    /// Method that allow to permanently remove user from game session.
    /// </summary>
    /// <param name="token">Join token</param>
    /// <param name="username">user username</param>
    /// <returns>Status code</returns>
    [HttpPost("[action]/{token}/{username}")]
    public async Task<IActionResult> BanFromSession(string token, string username)
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return StatusCode(403);
        
        return Json(await _service.BanFromSession(loggedUsername, token, username));
    }
    
    /// <summary>
    /// Method that allow to join user to session after being banned.
    /// </summary>
    /// <param name="token">Join token</param>
    /// <param name="username">user username</param>
    /// <returns>result</returns>
    [HttpPost("[action]/{token}/{username}")]
    public async Task<IActionResult> UnbanFromSession(string token, string username)
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return StatusCode(403);

        return Json(await _service.UnbanFromSession(loggedUsername, token, username));
    }

    /// <summary>
    /// Merhod that validating leaving game room
    /// </summary>
    /// <param name="connectionId">connection id</param>
    /// <param name="token">join token</param>
    /// <returns>result</returns>
    [HttpPost("[action]/{connectionId}/{token}")]
    public async Task<IActionResult> LeaveRoomJwt(string connectionId, string token)
    {
        var user = await _jwtService.ValidateToken(this);
        if (user == null) return StatusCode(403);
        
        return Json(await _service.LeaveRoom(user.Username, connectionId, token));
    }

    /// <summary>
    /// Method that creates quiz lobby
    /// </summary>
    /// <param name="connectionId">connection id</param>
    /// <param name="token">join token</param>
    /// <returns>result</returns>
    [HttpPost("[action]/{connectionId}/{token}")]
    public async Task<IActionResult> EstabilishedHostRoom(string connectionId, string token)
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return StatusCode(403);
        
        return Json(await _service.EstabilishedHostRoom(loggedUsername, connectionId, token));
    }

    /// <summary>
    /// Method that returns informations about quiz lobbyt
    /// </summary>
    /// <param name="token">join token</param>
    /// <returns>QuizLobbyInfoDto with data about quiz and host, 403 status is user is not logged</returns>
    [HttpPost("[action]/{token}")]
    public async Task<IActionResult> GetLobbyData(string token)
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return StatusCode(403);
        
        return Json(await _service.GetLobbyData(loggedUsername, token));
    }
    
    /// <summary>
    /// Method that send user anser to question
    /// </summary>
    /// <param name="connectionId">connection id</param>
    /// <param name="questionId">question id</param>
    /// <param name="answerId">answer id</param>
    /// <param name="isMultiAnswer">bool that check if question is multi answer</param>
    /// <returns>Status 200, 403 status is user is not logged</returns>
    [HttpPost("[action]/{connectionId}/{questionId}/{answerId}/{isMultiAnswer}")]
    public async Task<IActionResult> SendAnswer(string connectionId, string questionId, string answerId, bool isMultiAnswer)
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return StatusCode(403);
        
        await _service.SendAnswer(connectionId, questionId, answerId, isMultiAnswer);
        return StatusCode(200);
    }

    /// <summary>
    /// Method that validate sending uestion
    /// </summary>
    /// <param name="connectionId">connection id</param>
    /// <param name="questionId">question id</param>
    /// <param name="answerId">answer id</param>
    /// <param name="isMultiAnswer">bool that check if question is multi answer</param>
    /// <returns>Status 200, 403 status is user is not logged</returns>
    [HttpPost("[action]/{connectionId}/{questionId}/{answerId}/{isMultiAnswer}")]
    public async Task<IActionResult> SendAnswerJwt(string connectionId, string questionId, string answerId, bool isMultiAnswer)
    {
        var user = await _jwtService.ValidateToken(this);
        if (user == null) return StatusCode(403);
        
        await _service.SendAnswer(connectionId, questionId, answerId, isMultiAnswer);
        return StatusCode(200);
    }
}