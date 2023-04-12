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

    // tylko do testowania
    [HttpPost("[action]/{token}/{message}")]
    public async Task<IActionResult> SendMessage(string token, string message)
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null)
        {
            Response.StatusCode = 401;
            return Forbid();
        }
        await _service.SendMessage(loggedUsername, token, message);
        return Ok();
    }
}