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
    Task<List<UserEntity>> GetUsers();
    Task<List<UserEntity>> GetAdmins();
    Task<List<QuizEntity>> GetQuizList();
    Task QuizInfo(long id, AdminController controller);
    Task DelQuiz(long id, AdminController controller);
    Task<AddUserDto> GetUserData(long id, AdminController controller);
    Task UserInfo(long id, AdminController controller);
    Task UpdateUser(AddUserDtoPayload obj);
    Task SuspendUser(SuspendUserDtoPayload obj, string loggedUser);
    Task AddUser(AddUserDtoPayload obj, bool Admin);
    Task UnbanUser(long id, AdminController controller);
    Task DelUser(long id, AdminController controller, string loggedUser);

    Task CreateCoupons(CouponDtoPayload obj);
    Task<List<CouponDto>> GetCoupons();
    Task DeleteCoupon(string couponToDelete, AdminController controller);
}
