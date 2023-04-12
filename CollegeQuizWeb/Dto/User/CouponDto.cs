using System;
using System.ComponentModel.DataAnnotations;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Utils;

namespace CollegeQuizWeb.Dto.User;

public class CouponDto
{
    [Required(ErrorMessage = Lang.INVALID_COUPON_CODE_ERROR)]
    public string Coupon { get; set; }
    
    public bool IsUsed { get; set; }
    
    [Required(ErrorMessage = Lang.INVALID_COUPON_EXPIRING_DATE_ERROR)]
    public DateTime ExpiringAt { get; set; }
    
    [Required(ErrorMessage = Lang.INVALID_COUPON_EXTENSION_TIME_ERROR)]
    public int ExtensionTime { get; set; }
    
    [Required(ErrorMessage = Lang.INVALID_COUPON_AMOUNT_ERROR)]
    public int Amount { get; set; }
    
    [Required(ErrorMessage = Lang.INVALID_COUPON_AMOUNT_ERROR)]
    public int TypeOfSubscription { get; set; }
    
    public string CustomerName { get; set; }
}

public class CouponDtoPayload : AbstractControllerPayloader<AdminController>
{
    public CouponDto Dto { get; set; }
    
    public CouponDtoPayload(AdminController controllerReference) 
        : base(controllerReference) { }
}