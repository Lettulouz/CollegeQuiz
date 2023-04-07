using System.ComponentModel.DataAnnotations;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Dto.ChangePassword;
using CollegeQuizWeb.Utils;

namespace CollegeQuizWeb.Dto.User;

public class AttemptCouponRedeemDto
{
    [Required(ErrorMessage = Lang.INVALID_COUPON_CODE_ERROR)]
    public string Coupon { get; set; }
}

public class AttemptCouponRedeemPayloadDto : AbstractControllerPayloader<UserController>
{
    public AttemptCouponRedeemDto Dto { get; set; }
    
    public AttemptCouponRedeemPayloadDto(UserController controllerReference) 
        : base(controllerReference) { }
}