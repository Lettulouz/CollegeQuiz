using System.Threading.Tasks;
using CollegeQuizWeb.API.Services.QuizSession;
using CollegeQuizWeb.Jwt;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollegeQuizWeb.API.Controllers;

[Route("api/v1/dotnet/[controller]")]
public class QuizSessionAPIController : AbstractAPIController
{
    private readonly IQuizSessionAPIService _service;
    
    public QuizSessionAPIController(IQuizSessionAPIService service, IJwtService jwtService) : base(jwtService)
    {
        _service = service;
    }

    [HttpPost("[action]/{connectionId}/{token}")]
    public async Task<JsonResult> JoinRoom(string connectionId, string token)
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null)
        {
            Response.StatusCode = 401;
            return Json(new {});
        }
        return Json(await _service.JoinRoom(loggedUsername, connectionId, token));
    }

    [HttpPost("[action]/{connectionId}/{token}")]
    public async Task<JsonResult> JoinRoomJwt(string connectionId, string token)
    {
        var user = await _jwtService.ValidateToken(this);
        if (user == null) return Json(new {});
        
        return Json(await _service.JoinRoom(user.Username, connectionId, token));
    }
    
    [HttpPost("[action]/{connectionId}/{token}")]
    public async Task<JsonResult> LeaveRoom(string connectionId, string token)
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null)
        {
            Response.StatusCode = 401;
            return Json(new {});
        }
        return Json(await _service.LeaveRoom(loggedUsername, connectionId, token));
    }

    [HttpPost("[action]/{connectionId}/{token}")]
    public async Task<JsonResult> LeaveRoomJwt(string connectionId, string token)
    {
        var user = await _jwtService.ValidateToken(this);
        if (user == null) return Json(new {});
        
        return Json(await _service.LeaveRoom(user.Username, connectionId, token));
    }

    [HttpPost("[action]/{connectionId}/{token}")]
    public async Task<JsonResult> EstabilishedHostRoom(string connectionId, string token)
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null)
        {
            Response.StatusCode = 401;
            return Json(new {});
        }
        return Json(await _service.EstabilishedHostRoom(loggedUsername, connectionId, token));
    }

    [HttpPost("[action]/{token}")]
    public async Task<JsonResult> GetLobbyData(string token)
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null)
        {
            Response.StatusCode = 401;
            return Json(new {});
        }
        return Json(await _service.GetLobbyData(loggedUsername, token));
    }
    
    [HttpPost("[action]/{connectionId}/{questionId}/{answerId}/{isMultiAnswer}")]
    public async Task<IActionResult> SendAnswer(string connectionId, string questionId, string answerId, bool isMultiAnswer)
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null)
        {
            Response.StatusCode = 401;
            return Forbid();
        }
        await _service.SendAnswer(connectionId, questionId, answerId, isMultiAnswer);
        return Ok();
    }

    [HttpPost("[action]/{connectionId}/{questionId}/{answerId}")]
    public async Task<IActionResult> SendAnswerJwt(string connectionId, string questionId, string answerId, bool isMultiAnswer)
    {
        //var user = await _jwtService.ValidateToken(this);
        //if (user == null) return Forbid();
        
        await _service.SendAnswer(connectionId, questionId, answerId, isMultiAnswer);
        return Ok();
    }
}