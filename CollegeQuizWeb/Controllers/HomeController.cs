using System;
using System.Collections.Generic;
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
        
        return View();
    }

    public IActionResult Privacy() => View();
    
    public IActionResult Help()
    {
        return View();
    }
    
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

        using (var reader = new StreamReader(Request.Body))
       {
           var body = await reader.ReadToEndAsync();
           var order = JObject.Parse(body);
           var orderId = order["order"]["orderId"].ToString();
           var orderStatus = order["order"]["status"].ToString();
           var subscriptionName = order["order"]["products"][0]["name"].ToString();
           if(await _homeService.ChangePaymentStatus(orderStatus, orderId, subscriptionName))
               return Ok();
       }

        return new EmptyResult();
    }

    public  IActionResult Sandbox()
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