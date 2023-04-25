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
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return Redirect("/Auth/Login");

        ViewBag.Alert = Utilities.getSessionPropertyAndRemove(HttpContext, SessionKey.MY_QUIZES_ALERT)!;
        ViewBag.Quizes = await _service.GetPublicQuizes();
        
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Quizes(PublicQuizesDto obj)
    {
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
        await _service.PublicQuizInfo(id, this);
        return View();
    }

    public async void Share([FromRoute(Name = "id")] string token)
    {
        await _service.Share(token, this);
    }
}