using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CollegeQuizWeb.DbConfig;

public class OnStartInitializer : IHostedService
{
    private readonly IServiceScopeFactory scopeFactory;

    public OnStartInitializer(IServiceScopeFactory scopeFactory)
    {
        this.scopeFactory = scopeFactory;
    }
    
    private async Task ClearHostsTable()
    {
        using (var scope = scopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            // dbContext.QuizLobbies.RemoveRange(dbContext.QuizLobbies.ToList());
            await dbContext.SaveChangesAsync();
        }
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await ClearHostsTable();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await ClearHostsTable();
    }
}