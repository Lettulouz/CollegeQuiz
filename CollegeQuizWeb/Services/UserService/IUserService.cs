using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Dto.User;
using CollegeQuizWeb.Entities;

namespace CollegeQuizWeb.Services.UserService;

public interface IUserService
{
    /// <summary>
    /// Method that try redeem coupon for user
    /// </summary>
    /// <param name="obj">Dto with coupon data</param>
    Task AttemptCouponRedeem(AttemptCouponRedeemPayloadDto obj);
    
    /// <summary>
    /// Method that get user's coupon list
    /// </summary>
    /// <param name="userController">UserController instance</param>
    /// <param name="username">Current user Username</param>
    /// <returns>Coupons list</returns>
    Task<List<CouponListDto>> GetYourCouponsList(UserController userController, string username);
    
    /// <summary>
    /// Method that get user's payment history list
    /// </summary>
    /// <param name="userController">UserController instance</param>
    /// <param name="username">Current user Username</param>
    /// <returns>Payment history list</returns>
    Task<List<PaymentHistoryDto>> GetPaymentHistoryList(UserController userController, string username);
    
    /// <summary>
    /// Method that get all user information
    /// </summary>
    /// <param name="isLogged">Current user Username</param>
    /// <returns>All user information</returns>
    Task<ProfileDto> UserInfo(string isLogged);
    
    
    /// <summary>
    /// Method that get needed user infromation to edit profile
    /// </summary>
    /// <param name="isLogged">Current user Username</param>
    /// <returns>Needed user information</returns>
    Task<EditProfileDto> GetUserData(string isLogged);
    
    /// <summary>
    /// Method that update user information
    /// </summary>
    /// <param name="obj">Dto with user's account data</param>
    /// <param name="loggedUser">Current user Username</param>
    Task UpdateProfile(EditProfileDtoPayload obj, string loggedUser);
}