using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Dto;
using CollegeQuizWeb.Dto.User;
using CollegeQuizWeb.Entities;

namespace CollegeQuizWeb.Services.AdminService;

public interface IAdminService
{
    Task<List<UserEntity>> GetUsers();

    Task UserInfo(long id, AdminController controller);
    Task CreateCoupons(CouponDtoPayload obj);
}
