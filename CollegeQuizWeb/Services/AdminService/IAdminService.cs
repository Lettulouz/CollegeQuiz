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
    Task<List<UserEntity>> GetUsers();
    Task<AddUserDto> GetUserData(long id, AdminController controller);
    Task UserInfo(long id, AdminController controller);
    Task UpdateUser(AddUserDtoPayload obj);
    Task SuspendUser(SuspendUserDtoPayload obj);
    Task AddUser(AddUserDtoPayload obj);
    Task UnbanUser(long id, AdminController controller);
    Task DelUser(long id, AdminController controller);
    
    Task CreateCoupons(CouponDtoPayload obj);
    Task<List<CouponDto>> GetCoupons();
    Task DeleteCoupon(string couponToDelete, AdminController controller);
}
