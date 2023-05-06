using System;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Entities;
using Microsoft.AspNetCore.Http;

namespace CollegeQuizWeb.Utils;

public static class ExtendSubscription
{
    public static void AddSubscriptionTime(UserController controller, UserEntity userEntity, CouponEntity coupon, ref string message)
    {
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
                DateTime endDate;
                if (userEntity.CurrentStatusExpirationDate < DateTime.Today)
                    endDate = DateTime.Today.AddDays(coupon.ExtensionTime);
                else
                    endDate = userEntity.CurrentStatusExpirationDate.AddDays(coupon.ExtensionTime);
                userEntity.CurrentStatusExpirationDate = endDate;
                message = 
                    string
                        .Format(Lang.COUPON_ACTIVATED_MESSAGE, coupon.TypeOfSubscription, coupon.ExtensionTime, (endDate - DateTime.Today).TotalDays, endDate);
            }
            else if (coupon.TypeOfSubscription == 2)
            {
                userEntity.AccountStatus = coupon.TypeOfSubscription;
                int daysToAdd = 0;
                double calculateRemainingTime;
                if (userEntity.CurrentStatusExpirationDate < DateTime.Today)
                    calculateRemainingTime = 0;
                else
                    calculateRemainingTime =
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
            }
        }
    }
    
    public static void AddSubscriptionTime(UserEntity userEntity, int boughtSubscriptionType)
    {
        DateTime endDate = DateTime.Today.AddDays(30);
        if (userEntity.CurrentStatusExpirationDate < DateTime.Today) 
            userEntity.CurrentStatusExpirationDate = DateTime.Today;
        if (userEntity.AccountStatus == 1 && boughtSubscriptionType == 2)
        {
            double calculateRemainingTime;
                calculateRemainingTime =
                    (userEntity.CurrentStatusExpirationDate - DateTime.Today).TotalDays;
            
            int daysToAdd = (int) (calculateRemainingTime / 2);
                
            if(calculateRemainingTime % 2 != 0) daysToAdd++;

            endDate = endDate.AddDays(daysToAdd);
        }
        userEntity.AccountStatus = boughtSubscriptionType;
        userEntity.CurrentStatusExpirationDate = endDate;
    }
}