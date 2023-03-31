using System.Threading.Tasks;
using CollegeQuizWeb.Dto;
using CollegeQuizWeb.Entities;

namespace CollegeQuizWeb.Services.AuthService;

public interface IAuthService
{
    Task <DataDto> Register(UserEntity obj);
}