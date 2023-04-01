using System.Threading.Tasks;
using CollegeQuizWeb.Dto;
using CollegeQuizWeb.Entities;

namespace CollegeQuizWeb.Services.AuthService;

public interface IAuthService
{
    Task <RegisterDto> Register(RegisterDto obj);
    Task<bool> EmailExistsInDb(string email);
    Task<bool> UsernameExistsInDb(string username);
}