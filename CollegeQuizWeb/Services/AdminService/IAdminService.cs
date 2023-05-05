using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Dto;
using CollegeQuizWeb.Dto.User;
using CollegeQuizWeb.Dto.Admin;
using CollegeQuizWeb.Entities;

namespace CollegeQuizWeb.Services.AdminService;

public interface IAdminService
{
    /// <summary>
    ///  Method that get statistics from database
    /// about users, quizzes, coupons and subscriptions
    /// for index page in administrator panel
    /// </summary>
    /// <param name="controller">AdminController instance</param>
    Task GetStats(AdminController controller);

    /// <summary>
    /// Method that get list of users data
    /// for UsersList
    /// </summary>
    /// <returns>List of UserListDto</returns>
    Task<List<UserListDto>> GetUsers();
    
    /// <summary>
    /// Method that get data about user
    /// for EditProfile view
    /// </summary>
    /// <param name="id">user id</param>
    /// <param name="controller">AdminController instance</param>
    /// <returns>EditProfileDto</returns>
    Task<AddUserDto> GetUserData(long id, AdminController controller);
    
    /// <summary>
    /// Method that add new user and validate the data from form.
    /// Redirect to AdminList if user is administrator
    /// otherwise redirect to UsersList.
    /// </summary>
    /// <param name="obj">AddUserDtoPayload with user data</param>
    /// <param name="Admin">Boolean that determine if the user is Administrator</param>
    Task AddUser(AddUserDtoPayload obj, bool Admin);
    
    /// <summary>
    /// Method that suspend user permanently or temporary.
    /// Method doesn't allow to suspend its account.
    /// </summary>
    /// <param name="obj">SuspendUserDtoPayload with user data</param>
    /// <param name="loggedUser">Current user Username</param>
    Task SuspendUser(SuspendUserDtoPayload obj, string loggedUser);
    
    /// <summary>
    /// Method that unsuspend user account
    /// </summary>
    /// <param name="id">user id</param>
    /// <param name="controller">AdminController instance</param>
    Task UnbanUser(long id, AdminController controller);
    
    /// <summary>
    /// Method that update user's account and validate data from form.
    /// Method doesn't allow to remove administrator status from its account.
    /// </summary>
    /// <param name="obj">AddUserDtoPayload with user data</param>
    /// <param name="loggedUser">Current user Username</param>
    Task UpdateUser(AddUserDtoPayload obj, string loggedUser);
    
    /// <summary>
    /// Method that get user data fot UserProfile view.
    /// </summary>
    /// <param name="id">User if</param>
    /// <param name="controller">AdminController instance</param>
    Task UserInfo(long id, AdminController controller);
    
    /// <summary>
    /// Method  that resend email with activation link for unactive user.
    /// </summary>
    /// <param name="id">User idd</param>
    /// <param name="controller">AdminController instance</param>
    Task ResendEmail(long id, AdminController controller);
    
    /// <summary>
    /// Method that delete user account.
    /// Method doesn't allow to remove its account and users in game at the moment.
    /// </summary>
    /// <param name="id">User id</param>
    /// <param name="controller">AdminController instance</param>
    /// <param name="loggedUser">Current user Username</param>
    /// <returns></returns>
    Task DelUser(long id, AdminController controller, string loggedUser);
    
    /// <summary>
    /// Method that get list of administrators data.
    /// for AdminList
    /// </summary>
    /// <returns>List of UserListDto</returns>
    Task<List<UserListDto>> GetAdmins();
    
    /// <summary>
    /// Method that get list of quizzes for QuizList view
    /// </summary>
    Task<List<QuizListDto>> GetQuizList();
    
    /// <summary>
    /// Method that get quiz data for QuizView.
    /// </summary>
    /// <param name="id">Quiz id</param>
    /// <param name="controller">AdminController instance</param>
    Task QuizInfo(long id, AdminController controller);
    
    /// <summary>
    /// Method that remove quiz.
    /// Method doesn't allow to remove quiz played at the moment.
    /// </summary>
    /// <param name="id">Quiz id</param>
    /// <param name="controller">AdminController instance</param>
    Task DelQuiz(long id, AdminController controller);
    
    /// <summary>
    /// Method that lock quiz and make it unplayable.
    /// </summary>
    /// <param name="id">Quiz id</param>
    /// <param name="controller">AdminController instance</param>
    Task LockQuiz(long id, AdminController controller);
    
    /// <summary>
    /// Method that unlock quiz.
    /// </summary>
    /// <param name="id">Quiz id</param>
    /// <param name="controller">AdminController instance</param>
    Task UnlockQuiz(long id, AdminController controller);
    
    /// <summary>
    /// Method that remove category.
    /// </summary>
    /// <param name="id">Category id</param>
    /// <param name="controller">AdminController instance</param>
    Task DelCategory(long id, AdminController controller);
    
    /// <summary>
    /// Method that get categories list for CategoryList.
    /// </summary>
    /// <returns>Liest of CategoryListDto</returns>
    Task<List<CategoryListDto>> GetCategoryList();
    
    /// <summary>
    /// Method that allow to create one or more coupons.
    /// </summary>
    /// <param name="obj">CouponDtoPayload with coupon data</param>
    Task CreateCoupons(CouponDtoPayload obj);
    
    /// <summary>
    /// Method that allow to add new category.
    /// </summary>
    /// <param name="obj">CategoryListDtoPayload with category data</param>
    Task CreateCategory(CategoryListDtoPayload obj);
    
    /// <summary>
    /// Method that get coupons data for CouponList view.
    /// </summary>
    /// <returns>List of CouponDto</returns>
    Task<List<CouponDto>> GetCoupons();
    
    /// <summary>
    /// Method that allow to delete one or multiple coupons.
    /// </summary>
    /// <param name="couponToDelete">coupon or lisy of coupons</param>
    /// <param name="controller">AdminController instance</param>
    Task DeleteCoupon(string couponToDelete, AdminController controller);
    
    /// <summary>
    /// Method that get subscription types data for Subscription view.
    /// </summary>
    /// <returns>List of SubscriptionTypeDto</returns>
    Task<List<SubscriptionTypeDto>> GetSubscriptions();
    
    /// <summary>
    /// Method tha allow to update subscription type data.
    /// </summary>
    /// <param name="obj">SubscriptionTypeDtoPayload with subscription type data</param>
    Task UpdateSub(SubscriptionTypeDtoPayload obj);
}
