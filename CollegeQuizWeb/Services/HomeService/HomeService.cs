using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeQuizWeb.Dto;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Smtp;
using CollegeQuizWeb.SmtpViewModels;
using Microsoft.Extensions.Logging;

namespace CollegeQuizWeb.Services.HomeService;

public class HomeService : IHomeService
{
    private readonly ILogger<HomeService> _logger;
    private readonly ApplicationDbContext _context;
    private readonly ISmtpService _smtpService;

    public HomeService(ILogger<HomeService> logger, ApplicationDbContext context, ISmtpService smtpService)
    {
        _logger = logger;
        _context = context;
        _smtpService = smtpService;
    }

    public async Task<DataDto> GetString()
    {
        DataDto dataDto = new DataDto();

        //TestEntity testEntity = new TestEntity();
        //testEntity.Name = "Siema eniu tu doby mudzin z afryka";

        //await _context.AddAsync(testEntity);
        //await _context.SaveChangesAsync();

        TestSmtpViewModel viewModel = new TestSmtpViewModel()
        {
            Name = "Wprowadzona nazwa"
        };
        UserEmailOptions<TestSmtpViewModel> options = new UserEmailOptions<TestSmtpViewModel>()
        {
            TemplateName = TemplateName.TEST_TEMPLATE,
            ToEmails = new List<string>() { "example@gmail.com" },
            Subject = "To jest przykładowy temat",
            DataModel = viewModel
        };
        //await _smtpService.SendEmailMessage(options);
        
        dataDto.Test = "kocham disa"; 
        _logger.LogInformation("Siema eniu");
        return dataDto;
    }
}