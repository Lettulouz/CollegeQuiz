using System.Threading.Tasks;
using CollegeQuizWeb.API.Dto;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CollegeQuizWeb.API.Services.QuizSession;

public class QuizSessionAPIService : IQuizSessionAPIService
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<QuizSessionHub> _hubContext;

    public QuizSessionAPIService(IHubContext<QuizSessionHub> hubContext, ApplicationDbContext context)
    {
        _hubContext = hubContext;
        _context = context;
    }

    public async Task<SimpleResponseDto> ValidateAndJoinRoom(string loggedUsername, string connectionId, string sessionId)
    {
        await _hubContext.Groups.AddToGroupAsync(connectionId, sessionId);
        
        
        
        return new SimpleResponseDto();
    }

    public async Task<SimpleResponseDto> LeaveRoom(string loggedUsername, string connectionId, string sessionId)
    {
        await _hubContext.Groups.RemoveFromGroupAsync(connectionId, sessionId);
        
        
        
        return new SimpleResponseDto();
    }
}