using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CollegeQuizWeb.DbConfig;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CollegeQuizWeb.Hubs;

public class QuizUserSessionHub : Hub
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<QuizManagerSessionHub> _hubManager;

    public QuizUserSessionHub(ApplicationDbContext context, IHubContext<QuizManagerSessionHub> hubManager)
    {
        _context = context;
        _hubManager = hubManager;
    }

    public string GetConnectionId() => Context.ConnectionId;

    public async override Task OnDisconnectedAsync(Exception? exception)
    {
        var discUser = await _context.QuizSessionPartics
            .Include(u => u.UserEntity)
            .Include(u => u.QuizLobbyEntity)
            .FirstOrDefaultAsync(u => u.ConnectionId.Equals(Context.ConnectionId));
        if (discUser == null) return;
        
        var token = discUser.QuizLobbyEntity.Code;
        discUser.IsActive = false;
        _context.QuizSessionPartics.Update(discUser);
        await _context.SaveChangesAsync();

        var restOfPartic = _context.QuizSessionPartics
            .Include(p => p.QuizLobbyEntity)
            .Include(p => p.UserEntity)
            .Where(p => p.QuizLobbyEntity.Code.Equals(token));
        
        await _hubManager.Clients.Group(token).SendAsync("GetAllParticipants", JsonSerializer.Serialize(new {
            Connected = restOfPartic.Where(u => u.IsActive).Select(u => u.UserEntity.Username),
            Disconnected = restOfPartic.Where(u => !u.IsActive).Select(u => u.UserEntity.Username)
        }));
    }
}