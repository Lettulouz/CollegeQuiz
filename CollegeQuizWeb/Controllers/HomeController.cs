using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CollegeQuizWeb.Dto;
using CollegeQuizWeb.Dto.Home;
using Microsoft.AspNetCore.Mvc;
using CollegeQuizWeb.Models;
using CollegeQuizWeb.Services.HomeService;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Test = System.Web;

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
        string? status = HttpContext.Session.GetString(SessionKey.PAYMENT_TEST);
        ViewBag.status = status!;
        HttpContext.Session.Remove(SessionKey.PAYMENT_TEST);
        return View();
    }

    public IActionResult Privacy() => View();
    
    public IActionResult Regulation()
    {
        return View();
    }
    
    public async Task<IActionResult> Subscription(int? id=null)
    {
        if(id==null)
            Response.Redirect("/Home");
        
        ViewBag.TypeOfSubscription = id;
        var username = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        var subscriptionPaymentDto = await _homeService.GetUserData(username, this);
        return View(subscriptionPaymentDto);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Subscription(SubscriptionPaymentDto subscriptionPaymentDto)
    {
        var payloadDto = new SubscriptionPaymentDtoPayload(this) { Dto = subscriptionPaymentDto };
        await _homeService.MakePaymentForSubscription(payloadDto);
        return View(subscriptionPaymentDto);
    }

    
    [HttpPost]
    public async Task<IActionResult> ChangePaymentStatus()
    {
        HttpContext.Request.EnableBuffering();
       /* await _homeService.ChangePaymentStatus("test");
        //var value1 = HttpContext.Request.Body.Length.ToString();
        
        
        if (value1.Length > 25)
            value1 = "chuj";
        //if (value1.Length < 1) value1 = "test2";
        if (await _homeService.ChangePaymentStatus(value1))
            return Ok();
        return new EmptyResult();*/
       
       using (var reader = new StreamReader(Request.Body))
       {
           var body = await reader.ReadToEndAsync();
           var order = JObject.Parse(body);
           var test1 = order["order"]["orderId"].ToString();
           var test2 = order["order"]["status"].ToString();
           var test3 = order["order"]["products"][0]["name"].ToString();
           var testResult = test1 + "|" + test2 + "|" + test3;
           await _homeService.ChangePaymentStatus(testResult);
            

           return Ok();
       }
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