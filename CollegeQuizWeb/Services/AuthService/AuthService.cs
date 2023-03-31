using System.Threading.Tasks;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Dto;
using CollegeQuizWeb.Entities;
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

    public async Task<DataDto> Register(UserEntity obj)
    {
        UserEntity test = obj;
       /* test.FirstName = "Jacek";
        test.LastName = "Placek";
        test.Password = "Haselko123*";
        test.Email = "jakistamemail@wp.pl";
        test.RulesAccept = true;
        test.TeamID = 1;
        test.Username = "uzytkownik";
*/
      
       await _context.AddAsync(test);
       await _context.SaveChangesAsync();
           //TempData["success"] = "Kategoria utworzona pomyślnie";

           //return RedirectToAction("Index", "Category");
       
        await _context.AddAsync(test);
        await _context.SaveChangesAsync();
        // _context.Add(new UserEntity());

        //TestEntity testEntity = new TestEntity();
        //testEntity.Name = "Siema eniu tu doby mudzin z afryka";

        //await _context.AddAsync(testEntity);
        //await _context.SaveChangesAsync();
        return new DataDto();
    }
}
