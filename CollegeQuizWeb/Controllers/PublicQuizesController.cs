using System;
using System.Threading.Tasks;
using CollegeQuizWeb.Dto.PublicQuizes;
using CollegeQuizWeb.Services.PublicQuizesService;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollegeQuizWeb.Controllers;

public class PublicQuizesController : Controller
{
    private readonly IPublicQuizesService _service;

    public PublicQuizesController(IPublicQuizesService service)
    {
        _service = service;
    }
    
    [HttpGet]
    public async Task<IActionResult> Quizes()
    {
        await HttpContext.Session.CommitAsync();
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return Redirect("/Auth/Login");
        
        ViewBag.TokenMessage = HttpContext.Session.GetString(SessionKey.QUIZ_CODE_MESSAGE_REDEEM);
        ViewBag.Type = HttpContext.Session.GetString(SessionKey.QUIZ_CODE_MESSAGE_REDEEM_TYPE);
        HttpContext.Session.Remove(SessionKey.QUIZ_CODE_MESSAGE_REDEEM);
        HttpContext.Session.Remove(SessionKey.QUIZ_CODE_MESSAGE_REDEEM_TYPE);


        ViewBag.Alert = Utilities.getSessionPropertyAndRemove(HttpContext, SessionKey.MY_QUIZES_ALERT)!;
        ViewBag.Quizes = await _service.GetPublicQuizes();
        
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Quizes(PublicQuizesDto obj)
    {
        await HttpContext.Session.CommitAsync();
        var payloadDto = new PublicDtoPayLoad(this) { Dto = obj };
        if (payloadDto.Dto.Name == null)
        {
            ViewBag.Quizes = await _service.GetPublicQuizes();
        }
        else
        {
            ViewBag.Quizes = await _service.FilterQuizes(payloadDto);
        }
        return View(obj);
    }
    
    [HttpGet]
    public async Task<IActionResult> QuizPage([FromRoute(Name = "id")] long id)
    {
        await HttpContext.Session.CommitAsync();
        await _service.PublicQuizInfo(id, this);
        return View();
    }

    public async Task Share([FromRoute(Name = "id")] string token)
    {
        await HttpContext.Session.CommitAsync();
        await _service.Share(token, this);
    }
}