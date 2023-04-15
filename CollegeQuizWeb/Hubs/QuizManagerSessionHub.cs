using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CollegeQuizWeb.DbConfig;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CollegeQuizWeb.Hubs;

public class QuizManagerSessionHub : Hub
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<QuizUserSessionHub> _hubUserContext;

    public QuizManagerSessionHub(ApplicationDbContext context, IHubContext<QuizUserSessionHub> hubUserContext)
    {
        _context = context;
        _hubUserContext = hubUserContext;
    }
    
    public string GetConnectionId() => Context.ConnectionId;

    public async Task<int> INIT_GAME_SEQUENCER_P2P(int counter, string token)
    {
        await Clients.Group(token).SendAsync("INIT_GAME_SEQUENCER_P2P", counter);
        await _hubUserContext.Clients.Group(token).SendAsync("INIT_GAME_SEQUENCER_P2P", counter);
        
        var lobby = await _context.QuizLobbies.FirstOrDefaultAsync(l => l.Code.Equals(token));
        if (lobby == null) return counter;
       
        if (counter == 0) lobby.InGameScreen = "IN_GAME";
        else lobby.InGameScreen = "COUNTING_SCREEN";
        
        _context.QuizLobbies.Update(lobby);
        await _context.SaveChangesAsync();
        return counter;
    }
    
    public async Task START_GAME_P2P(string token)
    {
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

        await _hubUserContext.Clients.Group(token).SendAsync("START_GAME_P2P");
        
        foreach (var question in questions)
        {
            await _hubUserContext.Clients.Group(token).SendAsync("QUESTION_P2P",  JsonSerializer.Serialize(question));
        }
    }
    
    public async override Task OnDisconnectedAsync(Exception? exception)
    {
        var hostUser = await _context.QuizLobbies
            .Include(q => q.UserEntity)
            .FirstOrDefaultAsync(q => q.HostConnId.Equals(Context.ConnectionId));
        if (hostUser == null) return;
        
        await _hubUserContext.Clients.Group(hostUser.Code).SendAsync("OnDisconectedSession", "Host zakończył sesję.");
        
        var entities = _context.QuizSessionPartics
            .Include(q => q.QuizLobbyEntity)
            .Where(q => q.QuizLobbyEntity.Code.Equals(hostUser.Code));

        hostUser.IsEstabilished = false;
        hostUser.HostConnId = string.Empty;
        _context.QuizLobbies.Update(hostUser);
        
        if (entities.Count() > 0) _context.QuizSessionPartics.RemoveRange(entities);
        await _context.SaveChangesAsync();

    }
}