using System;
using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Dto.User;
using CollegeQuizWeb.Utils;
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
        
        var coupon = await _context.Coupons.FirstOrDefaultAsync(o => o.Token.Equals(obj.Dto.Coupon));

        if (coupon != null)
        {
            if (coupon.IsUsed)
            {
                controller.ModelState.AddModelError("Coupon", Lang.USED_COUPON_CODE_ERROR);
            }

            if (DateTime.Today > coupon.ExpiringAt)
            {
                controller.ModelState.AddModelError("Coupon", Lang.INACTIVE_COUPON_CODE_ERROR);
            }

            if (controller.ModelState.IsValid)
            {
                coupon.IsUsed = true;
                _context.Update(coupon);
                await _context.SaveChangesAsync();
            }
        }
        else
        {
            controller.ModelState.AddModelError("Coupon", Lang.INVALID_COUPON_CODE_ERROR);
        }
        //controller.Response.Redirect("/Home");
    }
}