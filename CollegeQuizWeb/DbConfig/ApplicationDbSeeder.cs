namespace CollegeQuizWeb.DbConfig;

public class ApplicationDbSeeder
{
    private readonly ApplicationDbContext _context;
    
    public ApplicationDbSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Seed()
    {
        if (!_context.Database.CanConnect()) return;
        // insert here asynchronyous methods for adding content to the database
    }
}