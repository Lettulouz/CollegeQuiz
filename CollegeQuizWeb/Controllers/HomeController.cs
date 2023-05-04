using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CollegeQuizWeb.Dto.Home;
using Microsoft.AspNetCore.Mvc;
using CollegeQuizWeb.Models;
using CollegeQuizWeb.Services.HomeService;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Test = System.Web;

namespace CollegeQuizWeb.Controllers;

public class HomeController : Controller
{
    private readonly IHomeService _homeService;
    
    /// <summary>
    /// Home controller, contains basic methods of page
    /// </summary>
    /// <param name="homeService">Home service interface</param>
    public HomeController(IHomeService homeService)
    { 
        _homeService = homeService;
    }

    /// <summary>
    /// Method that is being used to render home page view
    /// </summary>
    /// <returns>Home page View</returns>
    public async Task<IActionResult> Index()
    {
        await HttpContext.Session.CommitAsync();
        string? logoutUser = HttpContext.Session.GetString(SessionKey.USER_LOGOUT);
        ViewBag.Logout = logoutUser!;
        HttpContext.Session.Remove(SessionKey.USER_LOGOUT);
        
        return View();
    }

    /// <summary>
    /// Method that is being used to render rules view
    /// </summary>
    /// <returns>Rules view</returns>
    public IActionResult Rules()
    {
        return View();
    }
    
    /// <summary>
    /// Method that is being used to render privacy view
    /// </summary>
    /// <returns>Privacy view</returns>
    public IActionResult Privacy() => View();
    
    /// <summary>
    /// Method that is being used to render help view
    /// </summary>
    /// <returns>Help view</returns>
    public IActionResult Help()
    {
        return View();
    }

    /// <summary>
    /// Method that is being used to render subscription type view
    /// </summary>
    /// <param name="id">Type of subsciption</param>
    /// <returns>Subscription Views</returns>
    [HttpGet]
    public async Task<IActionResult> Subscription(int? id=null)
    {
        string? isUserLogged = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if(isUserLogged == null) return Redirect("/Auth/Login");
        await HttpContext.Session.CommitAsync();
        if(id==null)
            Response.Redirect("/Auth/Login");
        
        ViewBag.TypeOfSubscription = id!;
        var username = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (username.IsNullOrEmpty())
            return Redirect("/Auth/Login");
        var subscriptionPaymentDto = await _homeService.GetUserData(username!, this);
        ViewBag.SubscriptionMessage = HttpContext.Session.GetString(SessionKey.SUBSCRIPTION_MESSAGE)!;
        ViewBag.Type = HttpContext.Session.GetString(SessionKey.SUBSCRIPTION_MESSAGE_TYPE)!;
        HttpContext.Session.Remove(SessionKey.SUBSCRIPTION_MESSAGE);
        HttpContext.Session.Remove(SessionKey.SUBSCRIPTION_MESSAGE_TYPE);
        return View(subscriptionPaymentDto);
    }
    
    /// <summary>
    /// Method that is being used to make payment for chosen subscription
    /// </summary>
    /// <param name="subscriptionPaymentDto">Dto with subscription payment data</param>
    /// <returns>Subscription View</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Subscription(SubscriptionPaymentDto subscriptionPaymentDto)
    {
        string? isUserLogged = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if(isUserLogged == null) return Redirect("/Auth/Login");
        await HttpContext.Session.CommitAsync();
        var payloadDto = new SubscriptionPaymentDtoPayload(this) { Dto = subscriptionPaymentDto };
        await _homeService.MakePaymentForSubscription(payloadDto);
        return View(subscriptionPaymentDto);
    }

    /// <summary>
    /// Method that is being used by PayU Api to change payment status on server database
    /// </summary>
    /// <returns>Status code for PayU</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePaymentStatus()
    {
        await HttpContext.Session.CommitAsync();
        HttpContext.Request.EnableBuffering();

        using (var reader = new StreamReader(Request.Body))
       {
           var body = await reader.ReadToEndAsync();
           var order = JObject.Parse(body);
           if (order["order"] == null) return StatusCode(499);
           if (order["order"]!["orderId"] == null) return StatusCode(499);
           if (order["order"]!["status"] == null) return StatusCode(499);
           if (order["order"]!["products"] == null) return StatusCode(499);
           if (order["order"]!["products"]![0] == null) return StatusCode(499);
           if (order["order"]!["products"]![0]!["name"] == null) return StatusCode(499);
               
           var orderId = order["order"]!["orderId"]!.ToString();
           var orderStatus = order["order"]!["status"]!.ToString();
           var subscriptionName = order["order"]!["products"]![0]!["name"]!.ToString();
           if (await _homeService.ChangePaymentStatus(orderStatus, orderId, subscriptionName))
               return Ok();
           else
               return StatusCode(499);
       }
    }

    /// <summary>
    /// Method that is being used to display possible subscription types
    /// </summary>
    /// <returns>Choose subscription View</returns>
    public async Task<IActionResult> ChooseSubscription()
    {
        string? isUserLogged = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if(isUserLogged == null) return Redirect("/Auth/Login");
        await HttpContext.Session.CommitAsync();
        var subscriptions = await _homeService.GetSubscriptionTypes();
        return View(subscriptions);
    }
    

    /// <summary>
    /// Method that is being used to cover possible site errors
    /// </summary>
    /// <returns>View with error data</returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}