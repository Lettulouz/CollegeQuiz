using System.Linq;
using System.Threading.Tasks;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Dto;
using CollegeQuizWeb.Entities;
using CollegeQuizWeb.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CollegeQuizWeb.Services.AuthService;

public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<UserEntity> _passwordHasher;
    private readonly ISmtpService _smtpService;

    public AuthService(ILogger<AuthService> logger, ApplicationDbContext context, ISmtpService smtpService,
        IPasswordHasher<UserEntity> passwordHasher)
    {
        _logger = logger;
        _context = context;
        _smtpService = smtpService;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterDto> Register(RegisterDto obj)
    {
        RegisterDto test = obj;
        
        UserEntity test1 = new();
        test1.FirstName = test.FirstName;
        test1.LastName = test.LastName;
        test1.Username = test.Username;
        test1.Password = _passwordHasher.HashPassword(test1, test.Password);
        test1.Email = test.Email;
        test1.TeamID = test.TeamID;
        test1.RulesAccept = test.RulesAccept;
        
        await _context.AddAsync(test1);
        await _context.SaveChangesAsync();
       
        return test;
    }

    public async Task<bool> EmailExistsInDb(string email)
    {
        if (await _context.Users.FirstOrDefaultAsync(o => o.Email.Equals(email)) == null)
            return false;
        return true;
    }
    
    public async Task<bool> UsernameExistsInDb(string username)
    {
        if (await _context.Users.FirstOrDefaultAsync(o => o.Username.Equals(username)) == null)
            return false;
        return true;
    }
}
