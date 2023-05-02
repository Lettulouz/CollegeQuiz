using System.Threading;
using System.Threading.Tasks;
using CollegeQuizWeb.Dto.User;
using CollegeQuizWeb.Services.UserService;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ClearScript.JavaScript;

namespace CollegeQuizWeb.Controllers;

public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    public async Task<IActionResult> Profile()
    {
        await HttpContext.Session.CommitAsync();
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return Redirect("/Auth/Login");
        
        string? isLogged = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        ViewBag.Username = isLogged;
        
        string? UpdateAlert = HttpContext.Session.GetString(SessionKey.ACCOUNT_UPDATED);
        HttpContext.Session.Remove(SessionKey.ACCOUNT_UPDATED);
        ViewBag.UpdateAlert = UpdateAlert;
        
        var val = await _userService.UserInfo(isLogged);

        return View(val);
    }

    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        await HttpContext.Session.CommitAsync();
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return Redirect("/Auth/Login");

        var userProfile = await _userService.GetUserData(loggedUsername);
        
        return View(userProfile);
    }

    [HttpPost]
    public async Task<IActionResult> EditProfile(EditProfileDto obj)
    {
        await HttpContext.Session.CommitAsync();
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return Redirect("/Auth/Login");
        
        var payloadDto = new EditProfileDtoPayload(this) { Dto = obj };
        if (ModelState.IsValid)
        {
            await _userService.UpdateProfile(payloadDto,loggedUsername);
        }
        
        return View(obj);
    }
    
    
    public async Task<IActionResult> YourCoupons()
    {
        await HttpContext.Session.CommitAsync();
        var username = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (username == null) return Redirect("/Auth/Login");
        var dtoList = _userService.GetYourCouponsList(this, username);
        ViewBag.CouponList = dtoList.Result;

        return View();
    }
    
    public async Task<IActionResult> PaymentHistory()
    {
        await HttpContext.Session.CommitAsync();
        var username = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (username == null) return Redirect("/Auth/Login");
        var dtoList = _userService.GetPaymentHistoryList(this, username);
        ViewBag.PaymentHistoryList = dtoList.Result;

        return View();
    }
    
    public IActionResult AttemptCouponRedeem()
    {
        string? isLogged = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (isLogged == null) return Redirect("/Auth/Login");
        ViewBag.CouponMessage = HttpContext.Session.GetString(SessionKey.COUPON_CODE_MESSAGE_REDEEM)!;
        ViewBag.Type = HttpContext.Session.GetString(SessionKey.COUPON_CODE_MESSAGE_REDEEM_TYPE)!;
        HttpContext.Session.Remove(SessionKey.COUPON_CODE_MESSAGE_REDEEM);
        HttpContext.Session.Remove(SessionKey.COUPON_CODE_MESSAGE_REDEEM_TYPE);
        ViewBag.Username = isLogged;
        return View();
    }

    public async Task<IActionResult> SubscriptionAfterPaymentSelf()
    {
        await HttpContext.Session.CommitAsync();
        return View("SubscriptionPages/SubscriptionAfterPaymentSelf");
    }
    
    public async Task<IActionResult> SubscriptionAfterPaymentGift()
    {
        await HttpContext.Session.CommitAsync();
        return View("SubscriptionPages/SubscriptionAfterPaymentGift");
    }
    
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AttemptCouponRedeem(AttemptCouponRedeemDto attemptCouponRedeemDto)
    {
        await HttpContext.Session.CommitAsync();
        string? isLogged = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (isLogged == null) return Redirect("/Auth/Login");
        var payloadDto = new AttemptCouponRedeemPayloadDto(this) { Dto = attemptCouponRedeemDto };
        await _userService.AttemptCouponRedeem(payloadDto);
        return View(payloadDto.Dto);
    }
    
    
}