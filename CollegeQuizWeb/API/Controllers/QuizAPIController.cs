using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeQuizWeb.API.Dto;
using CollegeQuizWeb.API.Services.Quiz;
using CollegeQuizWeb.Sftp;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollegeQuizWeb.API.Controllers;

[Route("/api/v1/dotnet/[controller]")]
public class QuizAPIController : Controller
{
    private readonly IQuizAPIService _service;
    private readonly IAsyncSftpService _asyncSftpService;

    /// <summary>
    /// Contains methods for adding, getting questions, images and changins quiz names
    /// </summary>
    /// <param name="service"></param>
    /// <param name="asyncSftpService"></param>
    public QuizAPIController(IQuizAPIService service, IAsyncSftpService asyncSftpService)
    {
        _service = service;
        _asyncSftpService = asyncSftpService;
    }

    /// <summary>
    /// Method that get quiz questions
    /// </summary>
    /// <param name="id">quiz Id</param>
    /// <returns>quiz questions</returns>
    [HttpGet("[action]/{id}")]
    public async Task<IActionResult> GetQuizQuestions([FromRoute] long id)
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return StatusCode(403);
        
        return Json(await _service.GetQuizQuestions(id, loggedUsername, this));
    }
    
    /// <summary>
    /// Method that add questions to quiz
    /// </summary>
    /// <param name="id">quiz id</param>
    /// <param name="dto">Dto with questions</param>
    /// <returns>added questions</returns>
    [HttpPost("[action]/{id}")]
    public async Task<IActionResult> AddQuizQuestions([FromRoute] long id, [FromBody] AggregateQuestionReq2Dto dto)
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return StatusCode(403);
        
        return Json(await _service.AddQuizQuestions(id, dto, loggedUsername));
    }
    
    /// <summary>
    /// Method that change quiz name
    /// </summary>
    /// <param name="id">quiz Id</param>
    /// <param name="newName">quiz new name</param>
    /// <param name="dto">dto with Questions</param>
    /// <returns>result</returns>
    [HttpPost("[action]/{id}/{newName}")]
    public async Task<IActionResult> ChangeQuizName([FromRoute] long id, [FromRoute] string newName,
        [FromBody] AggregateQuestionsReqDto dto)
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return StatusCode(403);
        
        return Json(await _service.UpdateQuizName(id, newName, loggedUsername));
    }

    /// <summary>
    /// Method that get quiz image
    /// </summary>
    /// <param name="quizId">quiz Id</param>
    /// <param name="imgId">Image id</param>
    /// <param name="updatedAt">updated at</param>
    /// <returns>image</returns>
    [HttpGet("[action]/{quizId}/{imgId}/{updatedAt}")]
    public async Task<ActionResult> GetQuizImage([FromRoute(Name = "quizId")] long quizId,
        [FromRoute(Name = "imgId")] int imgId, [FromRoute(Name = "updatedAt")] string updatedAt)
    {
        byte[] image = await _asyncSftpService.GetQuizQuestionImageAsBytesArray(quizId, imgId, updatedAt);
        if (image.Length == 0) return NotFound();
        return File(image, "image/jpeg");
    }
    
    /// <summary>
    /// Methot that update quiz images
    /// </summary>
    /// <param name="id">quiz id</param>
    /// <param name="uploads">images</param>
    /// <returns>list of images</returns>
    [HttpPost("[action]/{id}")]
    public async Task<IActionResult> UpdateQuizImages([FromRoute] long id, [FromForm] List<IFormFile?> uploads)
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return StatusCode(403);
        
        return Json(await _service.UpdateQuizImages(id, uploads, loggedUsername, this));
    }
}