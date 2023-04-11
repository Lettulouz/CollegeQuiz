using System.Threading.Tasks;
using CollegeQuizWeb.API.Services.QuizSession;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollegeQuizWeb.API.Controllers;

[Route("api/v1/dotnet/[controller]")]
public class QuizSessionAPIController : Controller
{
    private readonly IQuizSessionAPIService _service;

    public QuizSessionAPIController(IQuizSessionAPIService service)
    {
        _service = service;
    }

    [HttpPost("[action]/{connectionId}/{sessionId}")]
    public async Task<JsonResult> ValidateAndJoinRoom(string connectionId, string sessionId)
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null)
        {
            Response.StatusCode = 401;
            return Json(new {});
        }
        return Json(await _service.ValidateAndJoinRoom(loggedUsername, connectionId, sessionId));
    }

    [HttpPost("[action]/{connectionId}/{sessionId}")]
    public async Task<JsonResult> LeaveRoom(string connectionId, string sessionId)
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null)
        {
            Response.StatusCode = 401;
            return Json(new {});
        }
        return Json(await _service.LeaveRoom(loggedUsername, connectionId, sessionId));
    }
    
    
}