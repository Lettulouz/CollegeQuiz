using CollegeQuizWeb.Dto.SharedQuizes;
using CollegeQuizWeb.Services.SharedQuizesService;
using Microsoft.AspNetCore.Mvc;

namespace CollegeQuizWeb.Controllers;

public class SharedQuizesController : Controller
{
    private readonly ISharedQuizesService _service;

    public SharedQuizesController(ISharedQuizesService service)
    {
        _service = service;
    }
    
    [HttpGet]
    public IActionResult Share()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Share(ShareTokenDto shareTokenDto)
    {
        var payloadDto = new ShareTokenPayloadDto(this) { Dto = shareTokenDto };
        
        return View();
    }
    
}