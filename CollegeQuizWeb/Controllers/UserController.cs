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
    
    public IActionResult Index()
    {
        string? isLogged = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        ViewBag.Username = isLogged;
        return View();
    }

    public async Task<IActionResult> YourCoupons()
    {
        var username = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        var dtoList = _userService.GetYourCouponsList(this, username);
        ViewBag.CouponList = dtoList.Result;

        return View();
    }
    
    public IActionResult AttemptCouponRedeem()
    {
        string? isLogged = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        ViewBag.CouponMessage = HttpContext.Session.GetString(SessionKey.COUPON_CODE_MESSAGE_REDEEM);
        ViewBag.Type = HttpContext.Session.GetString(SessionKey.COUPON_CODE_MESSAGE_REDEEM_TYPE);
        HttpContext.Session.Remove(SessionKey.COUPON_CODE_MESSAGE_REDEEM);
        HttpContext.Session.Remove(SessionKey.COUPON_CODE_MESSAGE_REDEEM_TYPE);
        ViewBag.Username = isLogged;
        return View();
    }

    public IActionResult SubscriptionAfterPaymentSelf()
    {
        return View();
    }
    
    public IActionResult SubscriptionAfterPaymentGift()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AttemptCouponRedeem(AttemptCouponRedeemDto attemptCouponRedeemDto)
    {
        var payloadDto = new AttemptCouponRedeemPayloadDto(this) { Dto = attemptCouponRedeemDto };
        await _userService.AttemptCouponRedeem(payloadDto);
        return View(payloadDto.Dto);
    }
}