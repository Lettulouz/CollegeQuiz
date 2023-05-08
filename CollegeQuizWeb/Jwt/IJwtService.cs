using System.Threading.Tasks;
using CollegeQuizWeb.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CollegeQuizWeb.Jwt;

public interface IJwtService
{
    /// <summary>
    /// Method that validate user
    /// </summary>
    /// <param name="controller">controller instance</param>
    /// <returns>user if validate otherwise nill</returns>
    Task<UserEntity?> ValidateToken(Controller controller);
    
    /// <summary>
    /// Method that generate user token
    /// </summary>
    /// <param name="username">user username</param>
    /// <returns>security token</returns>
    string GenerateToken(string username);
}