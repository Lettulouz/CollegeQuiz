using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Dto.User;
using CollegeQuizWeb.Entities;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

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
        
        string? loggedUser = controller.HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        if (loggedUser.IsNullOrEmpty())
        {
            controller.Response.Redirect("/Auth/Login");
            return;
        }

        var userEntity = _context.Users.FirstOrDefault(x => x.Username.Equals(loggedUser));
        if (userEntity == null)
        {
            controller.HttpContext.Session.Remove(SessionKey.IS_USER_LOGGED);
            controller.Response.Redirect("/Auth/Login");
            return;
        }

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
                
                string message = "";
                if (userEntity.AccountStatus == 0)
                {
                    userEntity.AccountStatus = coupon.TypeOfSubscription;
                    var endDate = DateTime.Today.AddDays(coupon.ExtensionTime);
                    userEntity.CurrentStatusExpirationDate = endDate;
                    message = 
                        string.Format(Lang.COUPON_ACTIVATED_MESSAGE, coupon.TypeOfSubscription, coupon.ExtensionTime, coupon.ExtensionTime, endDate);
                }
                else if (userEntity.AccountStatus == 1)
                {
                    if (coupon.TypeOfSubscription == 1)
                    {
                        var endDate = userEntity.CurrentStatusExpirationDate.AddDays(coupon.ExtensionTime);
                        userEntity.CurrentStatusExpirationDate = endDate;
                        message = 
                            string
                                .Format(Lang.COUPON_ACTIVATED_MESSAGE, coupon.TypeOfSubscription, coupon.ExtensionTime, (endDate - DateTime.Today).TotalDays, endDate);
                    }
                    else if (coupon.TypeOfSubscription == 2)
                    {
                        userEntity.AccountStatus = coupon.TypeOfSubscription;
                        int daysToAdd = 0;
                        var calculateRemainingTime =
                            (userEntity.CurrentStatusExpirationDate - DateTime.Today).TotalDays;
                        if (calculateRemainingTime % 2 == 0)
                            daysToAdd = (int) (calculateRemainingTime / 2);
                        else
                            daysToAdd = (int) (calculateRemainingTime / 2) + 1;
                        var endDate = DateTime.Today.AddDays(coupon.ExtensionTime + daysToAdd);
                        userEntity.CurrentStatusExpirationDate = endDate;
                        message = 
                            string.Format(Lang.COUPON_ACTIVATED_WITH_COMPENSATION_MESSAGE, coupon.TypeOfSubscription, 
                                coupon.ExtensionTime, calculateRemainingTime, daysToAdd, (endDate - DateTime.Today).TotalDays, endDate);
                    }
                }
                else if (userEntity.AccountStatus == 2)
                {
                    if (coupon.TypeOfSubscription == 2)
                    {
                        var endDate = userEntity.CurrentStatusExpirationDate.AddDays(coupon.ExtensionTime);
                        userEntity.CurrentStatusExpirationDate = endDate;
                        message = 
                            string
                                .Format(Lang.COUPON_ACTIVATED_MESSAGE, coupon.TypeOfSubscription, coupon.ExtensionTime, (endDate - DateTime.Today).TotalDays, endDate);
                    }
                    else
                    {
                        controller.HttpContext.Session
                            .SetString(SessionKey.COUPON_CODE_MESSAGE_REDEEM, Lang.COUPON_CODE_LOWER_LEVEL_ERROR);
                        controller.HttpContext.Session.SetString(SessionKey.COUPON_CODE_MESSAGE_REDEEM_TYPE,
                            "alert-danger");
                        controller.Response.Redirect("/User/AttemptCouponRedeem");
                        return;
                    }
                }
                _context.Update(coupon);
                _context.Update(userEntity);
                await _context.SaveChangesAsync();
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

    public async Task<List<CouponListDto>> GetYourCouponsList(UserController userController, string username)
    {
        var coupons =
            _context.GiftCouponsEntities
                .Include(q => q.CouponEntity)
                .Include(q => q.UserEntity)
                .Where(q => q.UserEntity.Username.Equals(username))
                .Select(o=> o.CouponEntity)
                .ToList();

        List<CouponListDto> couponListDtos = new();
        foreach (var coupon in coupons)
        {
            CouponListDto couponListDto = new();
            couponListDto.Coupon = coupon.Token;
            couponListDto.IsUsed = coupon.IsUsed;
            couponListDto.TypeOfSubscription = coupon.TypeOfSubscription;
            couponListDto.ExpiringAt = coupon.ExpiringAt;
            couponListDtos.Add(couponListDto);
        }
        return couponListDtos;
    }
    
    public async Task<List<PaymentHistoryDto>> GetPaymentHistoryList(UserController userController, string username)
    {
        var userId = _context.Users.FirstOrDefault(o => o.Username.Equals(username)).Id;
        var paymentHistoryList =
            _context.SubscriptionsPaymentsHistory
                .Where(o => o.UserId.Equals(userId))
                .OrderBy(o => o.CreatedAt);

        List<PaymentHistoryDto> paymentHistoryListDtos = new();
        foreach (var payment in paymentHistoryList)
        {
            PaymentHistoryDto paymentHistoryDto = new();
            paymentHistoryDto.Id = payment.Id;
            paymentHistoryDto.TypeOfSubscription = payment.Subscription;
            int priceH = (int)payment.Price/100;
            int priceL = (int)payment.Price%100;
            String priceString = priceH + "." + priceL;
            paymentHistoryDto.Price = priceString;
            paymentHistoryDto.OrderStatus = payment.OrderStatus;
            paymentHistoryListDtos.Add(paymentHistoryDto);
        }
        return paymentHistoryListDtos;
    }
}