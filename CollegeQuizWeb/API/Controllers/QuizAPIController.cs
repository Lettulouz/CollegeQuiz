using System.Collections.Generic;
using System.IO;
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
        return Json(await _service.GetQuizQuestions(id, loggedUsername, this));
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
    
    [HttpPost("[action]/{id}/{newName}")]
    public async Task<JsonResult> ChangeQuizName([FromRoute] long id, [FromRoute] string newName,
        [FromBody] AggregateQuestionsReqDto dto)
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null)
        {
            Response.StatusCode = 401;
            return Json(new {});
        }
        return Json(await _service.UpdateQuizName(id, newName, loggedUsername));
    }

    [HttpGet("[action]/{quizId}/{imgId}")]
    public ActionResult GetQuizImage([FromRoute(Name = "quizId")] long quizId, [FromRoute(Name = "imgId")] int imgId)
    {
        string ROOT_PATH = Directory.GetCurrentDirectory();
        string filePath = $"{ROOT_PATH}/_Uploads/QuizImages/{quizId}/question{imgId}.jpg";
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound();
        }
        byte[] file = System.IO.File.ReadAllBytes(filePath);
        return File(file, "image/jpeg");
    }
    
    [HttpPost("[action]/{id}")]
    public async Task<JsonResult> UpdateQuizImages([FromRoute] long id, [FromForm] List<IFormFile?> uploads)
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null)
        {
            Response.StatusCode = 401;
            return Json(new {});
        }
        return Json(await _service.UpdateQuizImages(id, uploads, loggedUsername, this));
    }
}