using System;
using System.Linq;
using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Dto.User;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CollegeQuizWeb.Services.UserService;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    
    private readonly ApplicationDbContext _context;

    public UserService(ILogger<UserService> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task AttemptCouponRedeem(AttemptCouponRedeemPayloadDto obj)
    {
        UserController controller = obj.ControllerReference;
        var coupon = _context.Coupons.FirstOrDefault(o => o.Token.Equals(obj.Dto.Coupon));

        if (coupon != null)
        {
            if (coupon.IsUsed)
            {
                controller.HttpContext.Session
                    .SetString(SessionKey.COUPON_CODE_MESSAGE_REDEEM, Lang.USED_COUPON_CODE_ERROR);
                controller.HttpContext.Session.SetString(SessionKey.COUPON_CODE_MESSAGE_REDEEM_TYPE, "alert-danger");
                controller.Response.Redirect("/User/AttemptCouponRedeem");
                return;
            }

            if (DateTime.Today > coupon.ExpiringAt)
            {
                controller.HttpContext.Session
                    .SetString(SessionKey.COUPON_CODE_MESSAGE_REDEEM, Lang.INACTIVE_COUPON_CODE_ERROR);
                controller.HttpContext.Session.SetString(SessionKey.COUPON_CODE_MESSAGE_REDEEM_TYPE, "alert-danger");
                controller.Response.Redirect("/User/AttemptCouponRedeem");
                return;
            }

            if (controller.ModelState.IsValid)
            {
                coupon.IsUsed = true;
                _context.Update(coupon);
                await _context.SaveChangesAsync();
                string message = 
                    string.Format(Lang.COUPON_ACTIVATED_MESSAGE, coupon.TypeOfSubscription, coupon.ExtensionTime);
                controller.HttpContext.Session.SetString(SessionKey.COUPON_CODE_MESSAGE_REDEEM, message);
                controller.HttpContext.Session.SetString(SessionKey.COUPON_CODE_MESSAGE_REDEEM_TYPE, "alert-success");
                controller.Response.Redirect("/User/AttemptCouponRedeem");
            }
        }
        else
        {
            controller.HttpContext.Session
                .SetString(SessionKey.COUPON_CODE_MESSAGE_REDEEM, Lang.INVALID_COUPON_CODE_ERROR);
            controller.HttpContext.Session.SetString(SessionKey.COUPON_CODE_MESSAGE_REDEEM_TYPE, "alert-danger");
            controller.Response.Redirect("/User/AttemptCouponRedeem");
        }
    }
}