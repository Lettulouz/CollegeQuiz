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

    /// <summary>
    /// Shared quizes controller, contains methods for quizes to be shared
    /// </summary>
    /// <param name="service">Shared quizes service interface</param>
    public SharedQuizesController(ISharedQuizesService service)
    {
        _service = service;
    }
    
    /// <summary>
    /// Method that is being used to render shared quizes view
    /// </summary>
    /// <returns>Shared quizes View</returns>
    public IActionResult Share()
    {
        string? isLogged = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (isLogged == null) return Redirect("/Auth/Login");
        ViewBag.TokenMessage = HttpContext.Session.GetString(SessionKey.QUIZ_CODE_MESSAGE_REDEEM)!;
        ViewBag.Type = HttpContext.Session.GetString(SessionKey.QUIZ_CODE_MESSAGE_REDEEM_TYPE)!;
        HttpContext.Session.Remove(SessionKey.QUIZ_CODE_MESSAGE_REDEEM);
        HttpContext.Session.Remove(SessionKey.QUIZ_CODE_MESSAGE_REDEEM_TYPE);
        ViewBag.Username = isLogged;
        return View();
    }
    
    /// <summary>
    /// Method that is being used to let user share quiz
    /// </summary>
    /// <param name="shareTokenDto">Token that is generated to share quizes</param>
    /// <returns>Shared quizes View</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Share(ShareTokenDto shareTokenDto)
    {
        await HttpContext.Session.CommitAsync();
        string? isLogged = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (isLogged == null) return Redirect("/Auth/Login");
        var payloadDto = new ShareTokenPayloadDto(this) { Dto = shareTokenDto };
        await _service.ShareQuizToken(payloadDto);
        return View(payloadDto.Dto);
    }
    
    /// <summary>
    /// Method that is being used share quiz
    /// </summary>
    /// <param name="id">Id of quiz that is going to be shared</param>
    /// <returns>Share page View</returns>
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