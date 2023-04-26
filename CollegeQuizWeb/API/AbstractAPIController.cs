using CollegeQuizWeb.Jwt;
using Microsoft.AspNetCore.Mvc;

namespace CollegeQuizWeb.API;

public abstract class AbstractAPIController : Controller
{
    protected readonly IJwtService _jwtService;

    protected AbstractAPIController(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }
}