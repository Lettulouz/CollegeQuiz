using System;
using System.Diagnostics;
using System.Threading.Tasks;
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
    
    public IActionResult Index() => View();
    
    public IActionResult Privacy() => View();
    
    public  IActionResult Regulation()
    {
        return View();
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