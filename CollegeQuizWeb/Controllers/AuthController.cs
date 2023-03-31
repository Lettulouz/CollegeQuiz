using System.Threading.Tasks;
using CollegeQuizWeb.Dto;
using CollegeQuizWeb.Services.AuthService;
using CollegeQuizWeb.Services.HomeService;
using Microsoft.AspNetCore.Mvc;

namespace CollegeQuizWeb.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    public IActionResult Register()
    {
        return View();
    }
}
