using System.Threading.Tasks;
using CollegeQuizWeb.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CollegeQuizWeb.Jwt;

public interface IJwtService
{
    Task<UserEntity?> ValidateToken(Controller controller);
    string GenerateToken(string username);
}