using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Entities;
using Microsoft.AspNetCore.Routing.Matching;
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
        var quiz = await _context.QuizLobbies
            .Include(q => q.QuizEntity)
            .FirstOrDefaultAsync(q => q.Code.Equals(token));

        var questions = await _context.Answers
            .Include(q => q.QuestionEntity)
            .Where(q => q.QuestionEntity.QuizId.Equals(quiz.QuizId))
            .Select(q => new
            {
                question = q.QuestionEntity.Name,
                answer = q.Name,
                time_min = q.QuestionEntity.TimeMin,
                time_sec = q.QuestionEntity.TimeSec
            })
            .GroupBy(q=>q.question)
            .Select(q=>new
            {
                question = q.Key,
                asnwer = q.Select(a => a.answer).ToList(),
                time_sec = q.Sum(a=>a.time_min*60+a.time_sec)
            }).ToListAsync();

        var delay = await _context.Answers
            .Include(q => q.QuestionEntity)
            .Where(q => q.QuestionEntity.QuizId.Equals(quiz.QuizId))
            .GroupBy(q => q.QuestionEntity.Name)
            .Select(q => q.Sum(a => a.QuestionEntity.TimeMin * 60 + a.QuestionEntity.TimeSec))
                .ToListAsync();

        await Clients.Group(token).SendAsync("START_GAME_P2P");


        foreach (var question in questions)
        {
            await Clients.Group(token).SendAsync("QUESTION_P2P",  JsonSerializer.Serialize(question));
        }
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