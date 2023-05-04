using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Dto.User;
using CollegeQuizWeb.Entities;

namespace CollegeQuizWeb.Services.UserService;

public interface IUserService
{
    /// <summary>
    /// Method that is being used to try redeem coupon for user
    /// </summary>
    /// <param name="obj">Dto with coupon data</param>
    Task AttemptCouponRedeem(AttemptCouponRedeemPayloadDto obj);
    
    /// <summary>
    /// Method that is being used to get user's coupon list
    /// </summary>
    /// <param name="userController">Pass data into usercontroller</param>
    /// <param name="username"> Logged user's username</param>
    /// <returns>Coupons list</returns>
    Task<List<CouponListDto>> GetYourCouponsList(UserController userController, string username);
    
    /// <summary>
    /// Method that is being used to get user's payment history list
    /// </summary>
    /// <param name="userController">Pass data into usercontroller</param>
    /// <param name="username">Logged user's username</param>
    /// <returns>Payment history list</returns>
    Task<List<PaymentHistoryDto>> GetPaymentHistoryList(UserController userController, string username);
    
    /// <summary>
    /// Method that is being used to get all user information
    /// </summary>
    /// <param name="isLogged">Logged user's username</param>
    /// <returns>All user information</returns>
    Task<ProfileDto> UserInfo(string isLogged);
    
    /// <summary>
    /// Method that is being used to get needed user infromation to edit profile
    /// </summary>
    /// <param name="isLogged">Logged user's username</param>
    /// <returns>Needed user information</returns>
    Task<EditProfileDto> GetUserData(string isLogged);
    
    /// <summary>
    /// Method that is being used to update user information
    /// </summary>
    /// <param name="obj">Dto with user's account data</param>
    /// <param name="loggedUser">Logged user's username</param>
    Task UpdateProfile(EditProfileDtoPayload obj, string loggedUser);
}