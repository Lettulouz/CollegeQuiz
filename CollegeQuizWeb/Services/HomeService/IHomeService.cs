using System.Threading.Tasks;
using CollegeQuizWeb.Dto;

namespace CollegeQuizWeb.Services.HomeService;

public interface IHomeService
{
    Task<DataDto> GetString();
}