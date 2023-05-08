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

    /// <summary>
    /// Auth controller, contains methods that are being used to authenticate user
    /// </summary>
    /// <param name="authService">Authenticate service interface</param>
    /// <param name="changePasswordService">Change password service interface</param>
    public AuthController(IAuthService authService, IChangePasswordService changePasswordService)
    {
        _authService = authService;
        _changePasswordService = changePasswordService;
    }
    
    /// <summary>
    /// Method that is being used to render login view
    /// </summary>
    /// <returns>Login View</returns>
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
    
    /// <summary>
    /// Method that is being used to log the user in
    /// </summary>
    /// <returns>Login View</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDto obj)
    {
        await HttpContext.Session.CommitAsync();
        var payloadDto = new LoginDtoPayload(this) { Dto = obj };
        
        if (ModelState.IsValid)
        {
            await _authService.Login(payloadDto);
        }
        
        return View(obj);
    }
    
    /// <summary>
    /// Method that is being used to render register view
    /// </summary>
    /// <returns>Register View</returns>
    [HttpGet] public IActionResult Register() => View();
    
    /// <summary>
    /// Method that is being used to register user
    /// </summary>
    /// <param name="registerDto">Dto with user data</param>
    /// <returns>Register View</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        var payloadDto = new RegisterDtoPayload(this) {Dto = registerDto};
        await _authService.Register(payloadDto);
        return View(payloadDto.Dto);
    }
    
    /// <summary>
    /// Method that is being used to render change user password view
    /// </summary>
    /// <param name="token">Token that was sent to user using email</param>
    /// <returns>Change password View</returns>
    [HttpGet]
    public async Task<IActionResult> ChangePassword([FromQuery(Name = "token")] string token)
    {
        await _changePasswordService.CheckBeforeChangePassword(token, this);
        return View();
    }
    
    /// <summary>
    /// Method that is being used to change user password
    /// </summary>
    /// <param name="token">Token that was sent to user using email</param>
    /// <param name="changePasswordDto">Dto with new password data</param>
    /// <returns>Change password View</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword([FromQuery(Name = "token")] string token, ChangePasswordDto changePasswordDto)
    {
        var payloadDto = new ChangePasswordPayloadDto(this) { Dto = changePasswordDto };
        await _changePasswordService.ChangePassword(token, payloadDto);
        return View(payloadDto.Dto);
    }

    /// <summary>
    /// Method that is being used to render attempt to change password view
    /// </summary>
    /// <returns>Attempt to change password View</returns>
    [HttpGet] public IActionResult AttemptChangePassword() => View();
    
    /// <summary>
    /// Method that is being used to process attempt to change password
    /// </summary>
    /// <param name="attemptChangePasswordDto">Dto with user email or login data</param>
    /// <returns>Attempt to change password View</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AttemptChangePassword(AttemptChangePasswordDto attemptChangePasswordDto)
    {
        var payloadDto = new AttemptChangePasswordPayloadDto(this) { Dto = attemptChangePasswordDto };
        await _changePasswordService.AttemptChangePassword(payloadDto);
        return View(payloadDto.Dto);
    }
    
    /// <summary>
    /// Method that is being used to confirm user's account
    /// </summary>
    /// <param name="token">Token that has been seen to user using email</param>
    [HttpGet]
    public async Task ConfirmAccount([FromQuery(Name = "token")] string token)
    {
        await HttpContext.Session.CommitAsync();
        await _authService.Activate(token, this);
    }

    /// <summary>
    /// Method that is being used to log user out 
    /// </summary>
    /// <returns>Redirect to Home Page View</returns>
    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        string logout = "logout";
        HttpContext.Session.SetString(SessionKey.USER_LOGOUT, logout);
        return RedirectToAction("Index", "Home");
    }
}
