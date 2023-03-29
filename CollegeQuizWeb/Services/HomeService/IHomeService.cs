using CollegeQuizWeb.Dto;

namespace CollegeQuizWeb.Services.HomeService;

public interface IHomeService
{
    Task<DataDto> GetString();
}