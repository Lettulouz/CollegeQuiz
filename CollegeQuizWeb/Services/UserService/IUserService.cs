using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Dto.User;
using CollegeQuizWeb.Entities;

namespace CollegeQuizWeb.Services.UserService;

public interface IUserService
{
    Task AttemptCouponRedeem(AttemptCouponRedeemPayloadDto obj);
    Task<List<CouponListDto>> GetYourCouponsList(UserController userController, string username);
    Task<List<PaymentHistoryDto>> GetPaymentHistoryList(UserController userController, string username);
    
    Task<ProfileDto> UserInfo(string isLogged);
}