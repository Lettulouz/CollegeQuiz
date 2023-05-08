using System;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Entities;
using Microsoft.AspNetCore.Http;

namespace CollegeQuizWeb.Utils;

public static class ExtendSubscription
{
    /// <summary>
    /// Method that add subscription to user account after making payment
    /// </summary>
    /// <param name="controller">User controller instance</param>
    /// <param name="userEntity">User entity</param>
    /// <param name="coupon">Coupon entity</param>
    /// <param name="message">message</param>
    /// <param name="subscriptionName">Sybscription type</param>
    public static void AddSubscriptionTime(UserController controller, UserEntity userEntity, CouponEntity coupon, ref string message, string subscriptionName)
    {
        DateTime endDate = DateTime.Today.AddDays(coupon.ExtensionTime);
        if (userEntity.CurrentStatusExpirationDate < DateTime.Now)
            userEntity.CurrentStatusExpirationDate = DateTime.Now;
        message = 
            string
                .Format(Lang.COUPON_ACTIVATED_MESSAGE, subscriptionName, coupon.ExtensionTime, (endDate - DateTime.Today).TotalDays, endDate);

        if (userEntity.AccountStatus == 1 && coupon.TypeOfSubscription == 2)
        {
            double calculateRemainingTime;
                calculateRemainingTime =
                    (userEntity.CurrentStatusExpirationDate - DateTime.Today).TotalDays;
            
            int daysToAdd = (int) (calculateRemainingTime / 2); 
            
            if(calculateRemainingTime % 2 != 0) daysToAdd++;
            
            endDate = endDate.AddDays(daysToAdd);
            message = 
                string.Format(Lang.COUPON_ACTIVATED_WITH_COMPENSATION_MESSAGE, subscriptionName, 
                    coupon.ExtensionTime, calculateRemainingTime, daysToAdd, (endDate - DateTime.Today).TotalDays, endDate);
            
        }
        else if (userEntity.AccountStatus == 2 && coupon.TypeOfSubscription != 2)
        {
            controller.HttpContext.Session
                .SetString(SessionKey.COUPON_CODE_MESSAGE_REDEEM, Lang.COUPON_CODE_LOWER_LEVEL_ERROR);
            controller.HttpContext.Session.SetString(SessionKey.COUPON_CODE_MESSAGE_REDEEM_TYPE,
                "alert-danger");
            controller.Response.Redirect("/User/AttemptCouponRedeem");
        }
        
        userEntity.AccountStatus = coupon.TypeOfSubscription;
        userEntity.CurrentStatusExpirationDate = endDate;
    }
    
    /// <summary>
    /// Method that extend subscription for user if already has subscription 
    /// </summary>
    /// <param name="userEntity">User entity</param>
    /// <param name="boughtSubscriptionType">Sybscription type id</param>
    public static void AddSubscriptionTime(UserEntity userEntity, int boughtSubscriptionType)
    {
        DateTime endDate = DateTime.Today.AddDays(30);
        if (userEntity.CurrentStatusExpirationDate < DateTime.Now) 
            userEntity.CurrentStatusExpirationDate = DateTime.Now;
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