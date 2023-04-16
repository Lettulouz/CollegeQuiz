using System.Threading.Tasks;
using CollegeQuizWeb.DbConfig;

namespace CollegeQuizWeb.Services.SharedQuizesService;

public class SharedQuizesService : ISharedQuizesService
{
    private readonly ApplicationDbContext _context;

    public SharedQuizesService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task ShareQuizToken()
    {
        
    }
}