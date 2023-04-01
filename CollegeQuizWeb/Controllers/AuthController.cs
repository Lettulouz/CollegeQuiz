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
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterDto obj)
    {
        if (await _authService.EmailExistsInDb(obj.Email))
        {
            ModelState.AddModelError("Email", Lang.EMAIL_ALREADY_EXIST);
        }
        if (await _authService.UsernameExistsInDb(obj.Username))
        {
            ModelState.AddModelError("Username", Lang.USERNAME_ALREADY_EXIST);
        }
        if (ModelState.IsValid)
        {
            await _authService.Register(obj);
            return RedirectToAction("Privacy", "Home");
        }
        return View(obj);
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
}
