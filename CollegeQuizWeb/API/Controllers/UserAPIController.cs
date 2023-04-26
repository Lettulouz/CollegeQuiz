using System.Threading.Tasks;
using CollegeQuizWeb.API.Services.User;
using CollegeQuizWeb.Jwt;
using Microsoft.AspNetCore.Mvc;

namespace CollegeQuizWeb.API.Controllers;

[Route("/api/v1/dotnet/[controller]")]
public class UserAPIController : AbstractAPIController
{
    private readonly IUserAPIService _apiService;

    public UserAPIController(IUserAPIService apiService, IJwtService jwtService) : base(jwtService)
    {
        _apiService = apiService;
    }
    
    [HttpGet("[action]")]
    public async Task<JsonResult> UserDetails()
    {
        var user = await _jwtService.ValidateToken(this);
        if (user == null) return Json(new {});
        
        return Json(_apiService.GetUserDetails(user));
    }
}