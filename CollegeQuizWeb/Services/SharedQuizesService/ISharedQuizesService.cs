using System.Threading.Tasks;

namespace CollegeQuizWeb.Services.SharedQuizesService;

public interface ISharedQuizesService
{
    Task ShareQuizToken();
}