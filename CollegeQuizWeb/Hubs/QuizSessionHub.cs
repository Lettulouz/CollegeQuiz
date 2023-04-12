using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CollegeQuizWeb.DbConfig;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CollegeQuizWeb.Hubs;

public class QuizSessionHub : Hub
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<QuizSessionHub> _logger;

    public QuizSessionHub(ApplicationDbContext context, ILogger<QuizSessionHub> logger)
    {
        _context = context;
        _logger = logger;
    }

    public string GetConnectionId() => Context.ConnectionId;

    public async override Task OnDisconnectedAsync(Exception? exception)
    {
        var discUser = await _context.QuizSessionPartics
            .Include(u => u.UserEntity)
            .Include(u => u.QuizLobbyEntity)
            .FirstOrDefaultAsync(u => u.ConnectionId.Equals(Context.ConnectionId));
        if (discUser == null) return;
        
        discUser.IsActive = false;
        await _context.SaveChangesAsync();

        var token = discUser.QuizLobbyEntity.Code;
        var restOfPartic = _context.QuizSessionPartics
            .Include(p => p.QuizLobbyEntity)
            .Include(p => p.UserEntity)
            .Where(p => p.QuizLobbyEntity.Code.Equals(token))
            .Select(p => p.UserEntity.Username)
            .ToList();
        await Clients.Group(token)
            .SendAsync("GetAllParticipants", JsonSerializer.Serialize(restOfPartic));
        
        _logger.LogInformation("User '{}' was leave quiz '{}' set to inactive", discUser.UserEntity.Username,
            discUser.QuizLobbyEntity.Code);
    }
}