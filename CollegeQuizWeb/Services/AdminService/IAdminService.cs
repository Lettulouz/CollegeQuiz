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
    Task GetStats(AdminController controller);

    Task<List<UserListDto>> GetUsers();
    Task<List<UserListDto>> GetAdmins();

    Task<List<QuizListDto>> GetQuizList();
    Task<List<CategoryListDto>> GetCategoryList();
    Task QuizInfo(long id, AdminController controller);
    Task DelQuiz(long id, AdminController controller);
    Task DelCategory(long id, AdminController controller);
    Task<AddUserDto> GetUserData(long id, AdminController controller);
    Task UserInfo(long id, AdminController controller);
    Task UpdateUser(AddUserDtoPayload obj, string loggedUser);
    Task SuspendUser(SuspendUserDtoPayload obj, string loggedUser);
    Task AddUser(AddUserDtoPayload obj, bool Admin);
    Task UnbanUser(long id, AdminController controller);
    Task DelUser(long id, AdminController controller, string loggedUser);

    Task ResendEmail(long id, AdminController controller);

    Task CreateCoupons(CouponDtoPayload obj);
    Task CreateCategory(CategoryListDtoPayload obj);
    Task<List<CouponDto>> GetCoupons();
    Task DeleteCoupon(string couponToDelete, AdminController controller);
}
