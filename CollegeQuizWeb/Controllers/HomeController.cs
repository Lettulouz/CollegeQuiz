using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CollegeQuizWeb.Dto;
using Microsoft.AspNetCore.Mvc;
using CollegeQuizWeb.Models;
using CollegeQuizWeb.Services.HomeService;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;

namespace CollegeQuizWeb.Controllers;

public class HomeController : Controller
{
    private readonly IHomeService _homeService;
    
    public HomeController(IHomeService homeService)
    { 
        _homeService = homeService;
    }

    public IActionResult Index()
    {
        string? logouUser = HttpContext.Session.GetString(SessionKey.USER_LOGOUT);
        ViewBag.Logout = logouUser!;
        HttpContext.Session.Remove(SessionKey.USER_LOGOUT);
        return View();
    }

    public IActionResult Privacy() => View();
    
    public  IActionResult Regulation()
    {
        return View();
    }
    
    public  IActionResult Sandbox()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sandbox2()
    {
        var temp = await _homeService.MakePayment();
        return Redirect(temp.RedirectUri);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="integerVal"></param>
    /// <param name="stringVal"></param>
    /// <param name="doubleVal"></param>
    /// <returns>Wartość wyjściowa</returns>
    /// <exception cref="ArgumentException"></exception>
    public string TestMethod(int integerVal, string stringVal, double doubleVal)
    {
        string testVal = "";
        try
        {
            testVal = integerVal.ToString() + stringVal + doubleVal.ToString();
        }
        catch
        {
            throw new ArgumentException("Upps coś poszło nie tak");
        }

        return testVal;
    }
    
}