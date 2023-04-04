using System.Threading.Tasks;
using CollegeQuizWeb.Dto;
using CollegeQuizWeb.Dto.ChangePassword;
using CollegeQuizWeb.Services.AuthService;
using CollegeQuizWeb.Services.ChangePasswordService;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollegeQuizWeb.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly IChangePasswordService _changePasswordService;

    public AuthController(IAuthService authService, IChangePasswordService changePasswordService)
    {
        _authService = authService;
        _changePasswordService = changePasswordService;
    }
    
    [HttpGet]
    public IActionResult Login()
    {
        string? changePassMessage = HttpContext.Session.GetString(SessionKey.CHANGE_PASSWORD_LOGIN_REDIRECT);
        HttpContext.Session.Remove(SessionKey.CHANGE_PASSWORD_LOGIN_REDIRECT);
        ViewBag.ChangePassMessage = changePassMessage!;
        
        string? activateMessage = HttpContext.Session.GetString(SessionKey.ACTIVATE_ACCOUNT_REDIRECT);
        string? activateViewBagType = HttpContext.Session.GetString(SessionKey.ACTIVATE_ACCOUNT_VIEWBAG_TYPE);
        ViewBag.ActivateAccount = activateMessage!;
        ViewBag.Type = activateViewBagType!;
        HttpContext.Session.Remove(SessionKey.ACTIVATE_ACCOUNT_REDIRECT);
        HttpContext.Session.Remove(SessionKey.ACTIVATE_ACCOUNT_VIEWBAG_TYPE);
        
        
        return View();
    }

    [HttpGet]
    public async Task ConfirmAccount([FromQuery(Name = "token")] string token)
    {
        await _authService.Activate(token, this);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDto obj)
    {

        var payloadDto = new LoginDtoPayload(this) { Dto = obj };
        
        if (ModelState.IsValid)
        {
            await _authService.Login(payloadDto);
            string? isLogged = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
            // return RedirectToAction("Privacy", "Home");
        }
        
        return View(obj);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        var payloadDto = new RegisterDtoPayload(this) {Dto = registerDto};
        await _authService.Register(payloadDto);
        return View(payloadDto.Dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AttemptChangePassword(AttemptChangePasswordDto attemptChangePasswordDto)
    {
        var payloadDto = new AttemptChangePasswordPayloadDto(this) { Dto = attemptChangePasswordDto };
        await _changePasswordService.AttemptChangePassword(payloadDto);
        return View(payloadDto.Dto);
    }

    [HttpGet]
    public async Task<IActionResult> ChangePassword([FromQuery(Name = "token")] string token)
    {
        await _changePasswordService.CheckBeforeChangePassword(token, this);
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword([FromQuery(Name = "token")] string token, ChangePasswordDto changePasswordDto)
    {
        var payloadDto = new ChangePasswordPayloadDto(this) { Dto = changePasswordDto };
        await _changePasswordService.ChangePassword(token, payloadDto);
        return View(payloadDto.Dto);
    }
    
    [HttpGet] public IActionResult Register() => View();
    [HttpGet] public IActionResult AttemptChangePassword() => View();

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}
