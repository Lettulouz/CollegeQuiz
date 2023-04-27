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

    [HttpGet("[action]/{id}")]
    public async Task<JsonResult> GetQuizQuestions([FromRoute] long id)
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null)
        {
            Response.StatusCode = 401;
            return Json(new {});
        }
        return Json(await _service.GetQuizQuestions(id, loggedUsername));
    }
    
    [HttpPost("[action]/{id}")]
    public async Task<JsonResult> AddQuizQuestions([FromRoute] long id, [FromBody] AggregateQuestionsReqDto dto)
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