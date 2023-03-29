using CollegeQuizWeb.Dto;
using CollegeQuizWeb.DbConfig;

namespace CollegeQuizWeb.Services.HomeService;

public class HomeService : IHomeService
{
    private readonly ILogger<HomeService> _logger;
    private readonly ApplicationDbContext _context;

    public HomeService(ILogger<HomeService> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<DataDto> GetString()
    {
        DataDto dataDto = new DataDto();

        //TestEntity testEntity = new TestEntity();
        //testEntity.Name = "Siema eniu tu doby mudzin z afryka";

        //await _context.AddAsync(testEntity);
        //await _context.SaveChangesAsync();
        
        dataDto.Test = "kocham disa"; 
        _logger.LogInformation("Siema eniu");
        return dataDto;
    }
}