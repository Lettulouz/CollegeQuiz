using System.Threading.Tasks;
using CollegeQuizWeb.Dto;

namespace CollegeQuizWeb.Services.AuthService;

public interface IAuthService
{
    Task Register(RegisterDtoPayload obj);
    Task Login(LoginDtoPayload obj);
    Task<bool> EmailExistsInDb(string email);
    Task<bool> UsernameExistsInDb(string username);
}