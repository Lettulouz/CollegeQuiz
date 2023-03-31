using System.Threading.Tasks;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Dto;
using CollegeQuizWeb.Smtp;
using Microsoft.Extensions.Logging;

namespace CollegeQuizWeb.Services.AuthService;

public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly ApplicationDbContext _context;
    private readonly ISmtpService _smtpService;

    public AuthService(ILogger<AuthService> logger, ApplicationDbContext context, ISmtpService smtpService)
    {
        _logger = logger;
        _context = context;
        _smtpService = smtpService;
    }

    public async Task Register()
    {
        // Example service method
    }
}
