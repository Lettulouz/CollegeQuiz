using CollegeQuizWeb.Dto;

namespace CollegeQuizWeb.Services.HomeService;

public class HomeService : IHomeService
{
    private readonly ILogger<HomeService> _logger;

    public HomeService(ILogger<HomeService> logger)
    {
        _logger = logger;
    }

    public DataDto GetString()
    {
        DataDto dataDto = new DataDto();
        dataDto.Test = "kocham disa"; 
        _logger.LogInformation("Siema eniu");
        return dataDto;
    }
}