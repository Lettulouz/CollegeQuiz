namespace CollegeQuizWeb.Services.HomeService;

public class HomeService : IHomeService
{
    private readonly ILogger<HomeService> _logger;

    public HomeService(ILogger<HomeService> logger)
    {
        _logger = logger;
    }

    public string GetString()
    {
        _logger.LogInformation("Siema eniu");
        return "kocham disa";
    }
}