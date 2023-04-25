using System;
using System.ComponentModel.DataAnnotations;

namespace CollegeQuizWeb.Dto.User;

public class CouponListDto
{
    public string Coupon { get; set; }
    
    public bool IsUsed { get; set; }
    
    public DateTime ExpiringAt { get; set; }

    public int TypeOfSubscription { get; set; }
}