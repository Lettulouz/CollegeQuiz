using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CollegeQuizWeb.API.Dto;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Entities;
using CollegeQuizWeb.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CollegeQuizWeb.API.Services.QuizSession;

public class QuizSessionAPIService : IQuizSessionAPIService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<QuizSessionAPIService> _logger;
    private readonly IHubContext<QuizSessionHub> _hubContext;
    
    public QuizSessionAPIService(IHubContext<QuizSessionHub> hubContext, ApplicationDbContext context,
        ILogger<QuizSessionAPIService> logger)
    {
        _hubContext = hubContext;
        _context = context;
        _logger = logger;
    }

    public async Task<JoinToSessionDto> JoinRoom(string loggedUsername, string connectionId, string token)
    {
        var quizLobby = await _context.QuizLobbies
            .Include(l => l.QuizEntity)
            .FirstOrDefaultAsync(l => l.Code.Equals(token, StringComparison.InvariantCultureIgnoreCase));

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username.Equals(loggedUsername));
        if (quizLobby == null) return new JoinToSessionDto()
        {
            IsGood = false,
            Message = $"Podany przez Ciebie kod <strong>{token}</strong> nie istnieje lub uległ przedawnieniu."
        };
        var alreadyJoined = await _context.QuizSessionPartics
            .Include(p => p.UserEntity)
            .Include(p => p.QuizLobbyEntity)
            .FirstOrDefaultAsync(p => p.UserEntity.Username.Equals(loggedUsername) && p.QuizLobbyEntity.Code.Equals(token));
        if (alreadyJoined == null)
        {
            await _context.QuizSessionPartics.AddAsync(new QuizSessionParticEntity()
            {
                ConnectionId = connectionId,
                IsActive = true,
                QuizLobbyEntity = quizLobby,
                UserEntity = user!
            });
        }
        else
        {
            alreadyJoined.ConnectionId = connectionId;
            alreadyJoined.IsActive = true;
        }
        var restOfPartic = _context.QuizSessionPartics
            .Include(p => p.QuizLobbyEntity)
            .Include(p => p.UserEntity)
            .Where(p => p.QuizLobbyEntity.Code.Equals(token))
            .Select(p => p.UserEntity.Username)
            .ToList();
        await _hubContext.Clients.Group(token)
            .SendAsync("GetAllParticipants", JsonSerializer.Serialize(restOfPartic));
        
        await _context.SaveChangesAsync();
        await _hubContext.Groups.AddToGroupAsync(connectionId, token);
        return new JoinToSessionDto()
        {
            IsGood = true,
            QuizName = quizLobby.QuizEntity.Name
        };
    }

    public async Task<SimpleResponseDto> LeaveRoom(string loggedUsername, string connectionId, string token)
    {
        var quizSessionPart = await _context.QuizSessionPartics
            .Include(q => q.UserEntity)
            .Include(p => p.QuizLobbyEntity)
            .FirstOrDefaultAsync(p => p.UserEntity.Username.Equals(loggedUsername)
                                      && p.QuizLobbyEntity.Code.Equals(token));
        if (quizSessionPart == null) return new SimpleResponseDto()
        {
            IsGood = false,
            Message = "Obecnie nie jesteś w wybranej grze."
        };
        quizSessionPart.IsActive = false;
        await _context.SaveChangesAsync();
        await _hubContext.Groups.RemoveFromGroupAsync(connectionId, token);

        var restOfPartic = _context.QuizSessionPartics
            .Include(p => p.QuizLobbyEntity)
            .Include(p => p.UserEntity)
            .Where(p => p.QuizLobbyEntity.Code.Equals(token))
            .Select(p => p.UserEntity.Username)
            .ToList();

        await _hubContext.Clients.Group(token)
            .SendAsync("GetAllParticipants", JsonSerializer.Serialize(restOfPartic));
        return new SimpleResponseDto()
        {
            IsGood = true,
            Message = "Wyszedłeś z sesji, aby wejść ponownie wprowadź kod quizu"
        };
    }

    // tylko do testowania
    public async Task SendMessage(string loggedUsername, string token, string message)
    {
        _logger.LogInformation($"{token} -> {message}");

        await _hubContext.Clients.Group(token)
            .SendAsync("ReceivedMessage", message);
    }
}