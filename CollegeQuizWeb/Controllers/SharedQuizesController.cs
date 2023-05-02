using System.Threading.Tasks;
using CollegeQuizWeb.Dto.SharedQuizes;
using CollegeQuizWeb.Services.SharedQuizesService;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollegeQuizWeb.Controllers;

public class SharedQuizesController : Controller
{
    private readonly ISharedQuizesService _service;

    public SharedQuizesController(ISharedQuizesService service)
    {
        _service = service;
    }
    
    public IActionResult Share()
    {
        string? isLogged = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        ViewBag.TokenMessage = HttpContext.Session.GetString(SessionKey.QUIZ_CODE_MESSAGE_REDEEM);
        ViewBag.Type = HttpContext.Session.GetString(SessionKey.QUIZ_CODE_MESSAGE_REDEEM_TYPE);
        HttpContext.Session.Remove(SessionKey.QUIZ_CODE_MESSAGE_REDEEM);
        HttpContext.Session.Remove(SessionKey.QUIZ_CODE_MESSAGE_REDEEM_TYPE);
        ViewBag.Username = isLogged;
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Share(ShareTokenDto shareTokenDto)
    {
        await HttpContext.Session.CommitAsync();
        var payloadDto = new ShareTokenPayloadDto(this) { Dto = shareTokenDto };
        await _service.ShareQuizToken(payloadDto);
        return View(payloadDto.Dto);
    }
    
    [HttpGet]
    public async Task<IActionResult> SharePage([FromRoute(Name = "id")] long id)
    {
        await HttpContext.Session.CommitAsync();
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return Redirect("/Auth/Login");
        
        await _service.ShareQuizInfo(id, this);
        
        
        return View();
    }
}