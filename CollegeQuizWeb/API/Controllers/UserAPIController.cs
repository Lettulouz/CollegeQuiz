using System.Threading.Tasks;
using CollegeQuizWeb.API.Services.User;
using CollegeQuizWeb.Jwt;
using Microsoft.AspNetCore.Mvc;

namespace CollegeQuizWeb.API.Controllers;

[Route("/api/v1/dotnet/[controller]")]
public class UserAPIController : AbstractAPIController
{
    private readonly IUserAPIService _apiService;

    /// <summary>
    /// Controller that is used to communicate with mobile app
    /// </summary>
    /// <param name="apiService">UserAPIService</param>
    /// <param name="jwtService">jwtService</param>
    public UserAPIController(IUserAPIService apiService, IJwtService jwtService) : base(jwtService)
    {
        _apiService = apiService;
    }
    
    /// <summary>
    /// Method that return user data for mobile app
    /// </summary>
    /// <returns>user data</returns>
    [HttpGet("[action]")]
    public async Task<JsonResult> UserDetails()
    {
        var user = await _jwtService.ValidateToken(this);
        if (user == null) return Json(new {});
        
        return Json(_apiService.GetUserDetails(user));
    }
}