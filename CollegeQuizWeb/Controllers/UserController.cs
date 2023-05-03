using System.Threading.Tasks;
using CollegeQuizWeb.Dto.User;
using CollegeQuizWeb.Services.UserService;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollegeQuizWeb.Controllers;

public class UserController : Controller
{
    private readonly IUserService _userService;

    /// <summary>
    /// User controller, contains methods that are connected with user and its profile
    /// </summary>
    /// <param name="userService">User service interface</param>
    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    /// <summary>
    /// Method that is being used to render profile view
    /// </summary>
    /// <returns>Profile View</returns>
    public async Task<IActionResult> Profile()
    {
        await HttpContext.Session.CommitAsync();
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return Redirect("/Auth/Login");
        
        ViewBag.Username = loggedUsername;
        
        string? UpdateAlert = HttpContext.Session.GetString(SessionKey.ACCOUNT_UPDATED);
        HttpContext.Session.Remove(SessionKey.ACCOUNT_UPDATED);
        ViewBag.UpdateAlert = UpdateAlert!;
        
        
        var val = await _userService.UserInfo(loggedUsername);

        return View(val);
    }

    /// <summary>
    /// Method that is being used to render edit profile view
    /// </summary>
    /// <returns>Edit profile View</returns>
    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        await HttpContext.Session.CommitAsync();
        string? loggedUsername = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUsername == null) return Redirect("/Auth/Login");

        var userProfile = await _userService.GetUserData(loggedUsername);
        
        return View(userProfile);
    }

    /// <summary>
    /// Method that is being used to edit user profile
    /// </summary>
    /// <param name="obj">Dto that contains new profile data</param>
    /// <returns>Edit profile View</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
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

    /// <summary>
    /// Method that is being used to render owned coupons list
    /// </summary>
    /// <returns>Owned coupons View</returns>
    public async Task<IActionResult> YourCoupons()
    {
        await HttpContext.Session.CommitAsync();
        var username = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (username == null) return Redirect("/Auth/Login");
        var dtoList = _userService.GetYourCouponsList(this, username);
        ViewBag.CouponList = dtoList.Result;

        return View();
    }
    
    /// <summary>
    /// Method that is being used to render payment history view
    /// </summary>
    /// <returns>Payment history View</returns>
    public async Task<IActionResult> PaymentHistory()
    {
        await HttpContext.Session.CommitAsync();
        var username = HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (username == null) return Redirect("/Auth/Login");
        var dtoList = _userService.GetPaymentHistoryList(this, username);
        ViewBag.PaymentHistoryList = dtoList.Result;

        return View();
    }
    
    /// <summary>
    /// Method that is being used to render after subscription view (bought as self)
    /// </summary>
    /// <returns>After self subscription View</returns>
    public async Task<IActionResult> SubscriptionAfterPaymentSelf()
    {
        await HttpContext.Session.CommitAsync();
        return View("SubscriptionPages/SubscriptionAfterPaymentSelf");
    }
    
    /// <summary>
    /// Method that is being used to render after subscription view (bought as gift)
    /// </summary>
    /// <returns>After gift subscription View</returns>
    public async Task<IActionResult> SubscriptionAfterPaymentGift()
    {
        await HttpContext.Session.CommitAsync();
        return View("SubscriptionPages/SubscriptionAfterPaymentGift");
    }
    
    /// <summary>
    /// Method that is being used to render coupon redeem view
    /// </summary>
    /// <returns>Coupon redeem View</returns>
    [HttpGet]
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
    
    /// <summary>
    /// Method that is being used to let user reedem coupon code
    /// </summary>
    /// <param name="attemptCouponRedeemDto">Dto that contains coupon data</param>
    /// <returns>Coupon redeem View</returns>
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