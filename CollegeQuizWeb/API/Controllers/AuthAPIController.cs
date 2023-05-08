using System.Threading.Tasks;
using CollegeQuizWeb.API.Dto;
using CollegeQuizWeb.API.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace CollegeQuizWeb.API.Controllers;

[Route("/api/v1/dotnet/[controller]")]
public class AuthAPIController : Controller
{
    private readonly IAuthAPIService _service;

    /// <summary>
    ///  API Auth controller, contains methods that are being used to authenticate user for mobile app
    /// </summary>
    /// <param name="service">API Authenticate service</param>
    public AuthAPIController(IAuthAPIService service)
    {
        _service = service;
    }

    /// <summary>
    /// Method that is being used to log the user in
    /// </summary>
    /// <param name="reqDto">Dto with user verification data</param>
    /// <returns>Json with aythentication result</returns>
    [HttpPost("[action]")]
    public async Task<JsonResult> LoginViaApi([FromBody] LoginReqDto reqDto)
    {
        return Json(await _service.LoginViaApi(this, reqDto));
    }

    /// <summary>
    ///  ethod that check if user is authenticated
    /// </summary>
    /// <returns>Json with aythentication result</returns>
    [HttpGet("[action]")]
    public async Task<JsonResult> TestAuthenticated()
    {
        return Json(await _service.TestAuthenticated(this));
    }
}