using System.Threading.Tasks;
using CollegeQuizWeb.Dto;
using CollegeQuizWeb.Entities;
using CollegeQuizWeb.Services.AuthService;
using CollegeQuizWeb.Services.HomeService;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Mvc;

namespace CollegeQuizWeb.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    public async Task<IActionResult> Index()
    {
        return View();
    }
    
    public async Task<IActionResult> Register()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterDto obj)
    {
        if (await _authService.EmailExistsInDb(obj.Email))
        {
            ModelState.AddModelError("Email", Lang.EmailAlreadyExistsError);
        }
        if (await _authService.UsernameExistsInDb(obj.Username))
        {
            ModelState.AddModelError("Username", Lang.UsernameAlreadyExistsError);
        }
        if (ModelState.IsValid)
        {
            await _authService.Register(obj);
            return RedirectToAction("Privacy", "Home");
        }
        
        return View(obj);
    }
}
