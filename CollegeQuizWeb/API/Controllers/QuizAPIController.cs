using System.Threading.Tasks;
using CollegeQuizWeb.API.Dto;
using CollegeQuizWeb.API.Services.Quiz;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollegeQuizWeb.API.Controllers;

[Route("/api/v1/dotnet/[controller]")]
public class QuizAPIController : Controller
{
    private readonly IQuizAPIService _service;

    public QuizAPIController(IQuizAPIService service)
    {
        _service = service;
    }

    [HttpGet("quiz-questions")]
    public async Task<JsonResult> GetQuizQuestions([FromQuery] long id)
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null)
        {
            Response.StatusCode = 401;
            return Json(new {});
        }
        return Json(await _service.GetQuizQuestions(id, loggedUsername));
    }
    
    [HttpPost("quiz-questions")]
    public async Task<JsonResult> AddQuizQuestions([FromQuery] long id, [FromBody] AggregateQuestionsReqDto dto)
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null)
        {
            Response.StatusCode = 401;
            return Json(new {});
        }
        return Json(await _service.AddQuizQuestions(id, dto, loggedUsername));
    }
}