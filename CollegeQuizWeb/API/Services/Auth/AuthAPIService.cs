using System.Threading.Tasks;
using CollegeQuizWeb.API.Dto;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Entities;
using CollegeQuizWeb.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeQuizWeb.API.Services.Auth;

public class AuthAPIService : IAuthAPIService
{
    
    private readonly ApplicationDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher<UserEntity> _passwordHasher;

    public AuthAPIService(ApplicationDbContext context, IPasswordHasher<UserEntity> passwordHasher, IJwtService jwtService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }
    
    public async Task<LoginResDto> LoginViaApi(Controller controller, LoginReqDto reqDto)
    {
        var userEntity = await _context.Users.FirstOrDefaultAsync(e =>
            e.Username.Equals(reqDto.UsernameOrEmail) || e.Email.Equals(reqDto.UsernameOrEmail));
        if (userEntity == null)
        {
            controller.Response.StatusCode = 401;
            return new LoginResDto() { IsGood = false };
        }
        var result = _passwordHasher.VerifyHashedPassword(userEntity, userEntity.Password, reqDto.Password);
        if (!result.Equals(PasswordVerificationResult.Success))
        {
            controller.Response.StatusCode = 401;
            return new LoginResDto() { IsGood = false };
        }
        return new LoginResDto()
        {
            IsGood = true,
            Token = _jwtService.GenerateToken(userEntity.Username),
        };
    }

    public async Task<object> TestAuthenticated(Controller controller)
    {
        var user = await _jwtService.ValidateToken(controller);
        
        if (user == null) return new { Authenticated = false };
        return new { Authenticated = true, user.Username };
    }
}