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

    public async Task<int> INIT_GAME_SEQUENCER_P2P(int counter, string token)
    {
        await Clients.Group(token).SendAsync("INIT_GAME_SEQUENCER_P2P", counter);
        return counter;
    }
    
    public async Task START_GAME_P2P(string token)
    {
        // TODO: odliczanie i serwowanie 1 pytania quizu
        await Clients.Group(token).SendAsync("START_GAME_P2P");
    }

    public async override Task OnDisconnectedAsync(Exception? exception)
    {
        var discUser = await _context.QuizSessionPartics
            .Include(u => u.UserEntity)
            .Include(u => u.QuizLobbyEntity)
            .FirstOrDefaultAsync(u => u.ConnectionId.Equals(Context.ConnectionId));
        if (discUser == null) // jeśli null to sprawdzanie czy host
        {
            var hostUser = await _context.QuizLobbies
                .Include(q => q.UserEntity)
                .FirstOrDefaultAsync(q => q.HostConnId.Equals(Context.ConnectionId));
            if (hostUser != null)
            {
                await Clients.Group(hostUser.Code).SendAsync("OnDisconectedSession", "Host zakończył sesję.");
                var entities = _context.QuizSessionPartics
                    .Include(q => q.QuizLobbyEntity)
                    .Where(q => q.QuizLobbyEntity.Code.Equals(hostUser.Code));
                if (entities.Count() > 0)
                {
                    _context.QuizSessionPartics.RemoveRange(entities);
                    await _context.SaveChangesAsync();
                }
                return;
            }
            return;
        }
        var token = discUser.QuizLobbyEntity.Code;
        discUser.IsActive = false;
        _context.QuizSessionPartics.Update(discUser);
        await _context.SaveChangesAsync();

        var restOfPartic = _context.QuizSessionPartics
            .Include(p => p.QuizLobbyEntity)
            .Include(p => p.UserEntity)
            .Where(p => p.QuizLobbyEntity.Code.Equals(token));
        
        await Clients.Group(token).SendAsync("GetAllParticipants", JsonSerializer.Serialize(new {
            Connected = restOfPartic.Where(u => u.IsActive).Select(u => u.UserEntity.Username),
            Disconnected = restOfPartic.Where(u => !u.IsActive).Select(u => u.UserEntity.Username)
        }));
        
        _logger.LogInformation("User '{}' was leave quiz '{}' set to inactive", discUser.UserEntity.Username,
            discUser.QuizLobbyEntity.Code);
    }
}