using System.Threading.Tasks;
using CollegeQuizWeb.API.Dto;
using CollegeQuizWeb.API.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace CollegeQuizWeb.API.Controllers;

[Route("/api/v1/dotnet/[controller]")]
public class AuthAPIController : Controller
{
    private readonly IAuthAPIService _service;

    public AuthAPIController(IAuthAPIService service)
    {
        _service = service;
    }

    [HttpPost("[action]")]
    public async Task<JsonResult> LoginViaApi([FromBody] LoginReqDto reqDto)
    {
        return Json(await _service.LoginViaApi(this, reqDto));
    }

    [HttpGet("[action]")]
    public async Task<JsonResult> TestAuthenticated()
    {
        return Json(await _service.TestAuthenticated(this));
    }
}