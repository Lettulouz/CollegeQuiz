using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using CollegeQuizWeb.Dto.Quiz;
using CollegeQuizWeb.Services.QuizService;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollegeQuizWeb.Controllers;

public class QuizController : Controller
{
    private readonly IQuizService _service;

    /// <summary>
    /// Quiz, contains methods for hosting, creating, checking already created quizes
    /// </summary>
    /// <param name="service">Quiz service interface</param>
    public QuizController(IQuizService service)
    {
        _service = service;
    }

    /// <summary>
    /// Method that is being used to render quizes (both created by user that invokes and shared)
    /// </summary>
    /// <returns>Quizes list View</returns>
    [HttpGet]
    public async Task<IActionResult> MyQuizes()
    {
        await HttpContext.Session.CommitAsync();
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return Redirect("/Auth/Login");
        
        ViewBag.Alert = Utilities.getSessionPropertyAndRemove(HttpContext, SessionKey.MY_QUIZES_ALERT)!;
        ViewBag.Quizes = await _service.GetMyQuizes(loggedUsername);
        ViewBag.QuizesShared = await _service.GetMyShareQuizes(loggedUsername);
        
        return View();
    }

    /// <summary>
    /// Method that is being used to render quiz preview 
    /// </summary>
    /// <param name="id">Id of quiz that is going to be previewed</param>
    /// <returns>Quiz preview View</returns>
    [HttpGet]
    public async Task<IActionResult> QuizPage([FromRoute(Name = "id")] long id)
    {
        await HttpContext.Session.CommitAsync();
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return Redirect("/Auth/Login");

        ViewBag.QuizData = await _service.GetQuizDetails(loggedUsername, id, this);
        
        return View();
    }

    /// <summary>
    /// Method that is being used to render add new quiz view
    /// </summary>
    /// <returns>Add new quiz View</returns>
    [HttpGet]
    public IActionResult AddQuiz()
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return Redirect("/Auth/Login");
        
        return View();
    }

    /// <summary>
    /// Method that is being used to add new quiz
    /// </summary>
    /// <param name="dto">Dto with new quiz data</param>
    /// <returns>Add new quiz View</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddQuiz(AddQuizDto dto)
    {
        await HttpContext.Session.CommitAsync();
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return Redirect("/Auth/Login");
        
        AddQuizDtoPayloader payloader = new AddQuizDtoPayloader(this) { Dto = dto };
        await _service.CreateNewQuiz(loggedUsername, payloader);
        return View(dto);
    }
    
    /// <summary>
    /// Method that is being used to prepare space for quiz lobby, that will be created in React
    /// </summary>
    /// <param name="quizId">Id of quiz that is going to be hosted</param>
    /// <returns>Quiz lobby View</returns>
    [HttpGet]
    public async Task<IActionResult> QuizLobby([FromRoute(Name = "id")] long quizId)
    {
        await HttpContext.Session.CommitAsync();
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return Redirect("/Auth/Login");

        if (await _service.CreateQuizCode(this, loggedUsername, quizId)) return Redirect("/Quiz/MyQuizes");
        
        Bitmap test = _service.GenerateQRCode(this, ViewBag.Code);
        MemoryStream ms = new MemoryStream();
        test.Save(ms, ImageFormat.Jpeg);
        byte[] byteImage = ms.ToArray();
        ViewBag.ImageBtm = Convert.ToBase64String(byteImage);
        ms.Close();
        return View();
    }

    /// <summary>
    /// Method that is being used to prepare quiz session view for React
    /// </summary>
    /// <returns>Quiz session View</returns>
    [HttpGet]
    public IActionResult QuizSession()
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return Redirect("/Auth/Login");
        return View();
    }

    /// <summary>
    /// Method that is being used to delete quiz
    /// </summary>
    /// <param name="quizId">Id of quiz that is going to be deleted</param>
    /// <returns>Redirect to MyQuizes</returns>
    [HttpGet]
    public async Task<IActionResult> DeleteQuiz([FromRoute(Name = "id")] long quizId)
    {
        await HttpContext.Session.CommitAsync();
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return Redirect("/Auth/Login");

        await _service.DeleteQuiz(quizId, loggedUsername, this);
        return Redirect("/Quiz/MyQuizes");
    }

    /// <summary>
    /// Method that is being used to delete connection with shared quiz
    /// </summary>
    /// <param name="quizId">Id of quiz that is going to be deleted</param>
    /// <returns>Redirect to MyQuizes</returns>
    [HttpGet]
    public async Task<IActionResult> DeleteSharedQuiz([FromRoute(Name = "id")] long quizId)
    {
        await HttpContext.Session.CommitAsync();
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return Redirect("/Auth/Login");

        await _service.DeleteSharedQuiz(quizId, loggedUsername, this);
        return Redirect("/Quiz/MyQuizes");
    }
}