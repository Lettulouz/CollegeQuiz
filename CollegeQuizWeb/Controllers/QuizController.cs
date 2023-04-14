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

    public QuizController(IQuizService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> MyQuizes()
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return Redirect("/Auth/Login");
        
        ViewBag.Alert = Utilities.getSessionPropertyAndRemove(HttpContext, SessionKey.MY_QUIZES_ALERT)!;
        ViewBag.Quizes = await _service.GetMyQuizes(loggedUsername);
        
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> QuizPage([FromRoute(Name = "id")] long id)
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return Redirect("/Auth/Login");

        ViewBag.QuizData = await _service.GetQuizDetails(loggedUsername, id, this);
        
        return View();
    }

    [HttpGet]
    public IActionResult AddQuiz()
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return Redirect("/Auth/Login");
        
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddQuiz(AddQuizDto dto)
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return Redirect("/Auth/Login");
        
        AddQuizDtoPayloader payloader = new AddQuizDtoPayloader(this) { Dto = dto };
        await _service.CreateNewQuiz(loggedUsername, payloader);
        return View(dto);
    }
    
    [HttpGet]
    public async Task<IActionResult> QuizLobby([FromRoute(Name = "id")] long quizId)
    {
        await _service.CreateQuizCode(this, quizId);
        Bitmap test = _service.GenerateQRCode(this, ViewBag.Code);
        MemoryStream ms = new MemoryStream();
        test.Save(ms, ImageFormat.Jpeg);
        byte[] byteImage = ms.ToArray();
        ViewBag.ImageBtm = Convert.ToBase64String(byteImage);
        return View();
    }

    [HttpGet]
    public IActionResult QuizSession()
    {
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return Redirect("/Auth/Login");
        return View();
    }
    
    [HttpGet] public IActionResult InGameQuestion() => View();
}