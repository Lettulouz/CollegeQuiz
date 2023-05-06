using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.Dto;

namespace CollegeQuizWeb.Services.AuthService;

public interface IAuthService
{
    /// <summary>
    /// Method that register new user
    /// </summary>
    /// <param name="obj">RegisterDtoPayload with new user's account data</param>
    Task Register(RegisterDtoPayload obj);
    
    /// <summary>
    /// Method that login user
    /// </summary>
    /// <param name="obj">LoginDtoPayload with user's login or email and password data</param>
    Task Login(LoginDtoPayload obj);
    
    /// <summary>
    /// Method that check if email exist in database
    /// </summary>
    /// <param name="email">New user email</param>
    /// <returns>True or false</returns>
    Task<bool> EmailExistsInDb(string email);
    
    /// <summary>
    /// Method that check if username exist in database
    /// </summary>
    /// <param name="username">New user username</param>
    /// <returns>True or false</returns>
    Task<bool> UsernameExistsInDb(string username);
    
    /// <summary>
    /// Method that active account 
    /// </summary>
    /// <param name="token">token</param>
    /// <param name="controller">AuthController instance</param>
    Task Activate(string token, AuthController controller);
}