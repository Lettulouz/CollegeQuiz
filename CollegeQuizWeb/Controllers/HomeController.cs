using System.Diagnostics;
using CollegeQuizWeb.Dto;
using Microsoft.AspNetCore.Mvc;
using CollegeQuizWeb.Models;
using CollegeQuizWeb.Services.HomeService;

namespace CollegeQuizWeb.Controllers;

public class HomeController : Controller
{
    private readonly IHomeService _homeService;
    
    public HomeController(IHomeService homeService)
    { 
        _homeService = homeService;
    }

    public async Task<IActionResult> Index()
    {
        DataDto dataDto = await _homeService.GetString();
        return View(dataDto);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}