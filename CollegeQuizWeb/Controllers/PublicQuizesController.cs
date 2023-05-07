using System.Threading.Tasks;
using CollegeQuizWeb.Dto.PublicQuizes;
using CollegeQuizWeb.Services.PublicQuizesService;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CollegeQuizWeb.Controllers;

public class PublicQuizesController : Controller
{
    private readonly IPublicQuizesService _service;

    /// <summary>
    /// Public quizes controller, contains methods for quizes with public flag
    /// </summary>
    /// <param name="service">Public quizes service interface</param>
    public PublicQuizesController(IPublicQuizesService service)
    {
        _service = service;
    }
    
    /// <summary>
    /// Method that is being used to render public quizes View
    /// </summary>
    /// <returns>Public quizes view</returns>
    [HttpGet]
    public async Task<IActionResult> Quizes()
    {
        await HttpContext.Session.CommitAsync();
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return Redirect("/Auth/Login");
        
        ViewBag.TokenMessage = HttpContext.Session.GetString(SessionKey.QUIZ_CODE_MESSAGE_REDEEM)!;
        ViewBag.Type = HttpContext.Session.GetString(SessionKey.QUIZ_CODE_MESSAGE_REDEEM_TYPE)!;
        HttpContext.Session.Remove(SessionKey.QUIZ_CODE_MESSAGE_REDEEM);
        HttpContext.Session.Remove(SessionKey.QUIZ_CODE_MESSAGE_REDEEM_TYPE);


        ViewBag.Alert = Utilities.getSessionPropertyAndRemove(HttpContext, SessionKey.MY_QUIZES_ALERT)!;

        if (HttpContext.Session.GetString(SessionKey.CATEGORY_FILTER) != "true")
        {
            ViewBag.Quizes = await _service.GetPublicQuizes();
        }
        
        await _service.Categories(this);

        return View();
    }

    /// <summary>
    /// Method that is being used to render public quizes view with filter
    /// </summary>
    /// <param name="obj">Way the list has to be filtered</param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Quizes(PublicQuizesDto obj)
    {
        await HttpContext.Session.CommitAsync();
        string? isUserLogged = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if(isUserLogged == null) return Redirect("/Auth/Login");
        var payloadDto = new PublicDtoPayLoad(this) { Dto = obj };
        if (payloadDto.Dto.Name.IsNullOrEmpty())
        {
            ViewBag.Quizes = await _service.GetPublicQuizes();
        }
        else
        {
            ViewBag.Quizes = await _service.FilterQuizes(payloadDto);
        }
        return View(obj);
    }
    
    /// <summary>
    /// Method that is being used to render quiz page view
    /// </summary>
    /// <param name="id">Id of quiz that is going to be shown</param>
    /// <returns>Quiz View</returns>
    [HttpGet]
    public async Task<IActionResult> QuizPage([FromRoute(Name = "id")] long id)
    {
        await HttpContext.Session.CommitAsync();
        string? isUserLogged = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if(isUserLogged == null) return Redirect("/Auth/Login");
        await _service.PublicQuizInfo(id, this);
        return View();
    }

    /// <summary>
    /// Method that is being used to assing public quiz
    /// </summary>
    /// <param name="token">Quiz generated hash</param>
    public async Task Share([FromRoute(Name = "id")] string token)
    {
        await HttpContext.Session.CommitAsync();
        string? isUserLogged = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if(isUserLogged == null)
        {
            HttpContext.Response.Redirect("/Auth/Login");
            return;
        }
        await _service.Share(token, this);
    }
    
    // public async Task Filter(string[] tags)
    // {
    //     string[] board = tags;
    //     Response.Redirect("/PublicQuizes/Quizes");
    //     ViewBag.Quizes = await _service.Filter(this, board);
    // }
}