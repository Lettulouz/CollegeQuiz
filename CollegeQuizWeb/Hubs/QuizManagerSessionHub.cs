using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Dto.Quiz;
using CollegeQuizWeb.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sprache;

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
        Console.WriteLine("punkt testowy 1");
        var quiz = await _context.QuizLobbies
            .Include(q => q.QuizEntity)
            .FirstOrDefaultAsync(q => q.Code.Equals(token));

        Console.WriteLine("punkt testowy 2");
        var questions = await _context.Answers
            .Include(q => q.QuestionEntity)
            .Where(q => q.QuestionEntity.QuizId.Equals(quiz.QuizId))
            .Select(q => new
            {
                questionId = q.QuestionEntity.Index,
                question = q.QuestionEntity.Name,
                answers = q.Name,
                time_min = q.QuestionEntity.TimeMin,
                time_sec = q.QuestionEntity.TimeSec
            })
            .GroupBy(q=>q.questionId)
            .Select(q=>new
            {
                questionId = q.Select(a => a.questionId).Distinct().FirstOrDefault(),
                question = q.Select(a => a.question).Distinct().FirstOrDefault(),
                answers = q.Select(a => a.answers).ToList(),
                time_sec = q.Select(a => a.time_min * 60 + a.time_sec).Distinct().Sum()
            }).ToListAsync();
        
        Console.WriteLine("punkt testowy 3");
        await _hubUserContext.Clients.Group(token).SendAsync("START_GAME_P2P");

        Console.WriteLine("punkt testowy 4");
        if(quiz.CurrentQuestion>=questions.Count) return;
        var question = questions[quiz.CurrentQuestion];
        
        int timer;
        
        await _hubUserContext.Clients.Group(token).SendAsync("QUESTION_P2P",  JsonSerializer.Serialize(question));
        Console.WriteLine("punkt testowy 5");
        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken token2 = cts.Token;
        timer = question.time_sec;
        var periodicTimer= new PeriodicTimer(TimeSpan.FromSeconds(1));
        cts.CancelAfter(TimeSpan.FromSeconds(question.time_sec));
        Console.WriteLine("punkt testowy 6");
        while (!cts.IsCancellationRequested)
        {
            if(await periodicTimer.WaitForNextTickAsync(token2))
            {
                timer--;
                await _hubUserContext.Clients.Group(token).SendAsync("QUESTION_TIMER_P2P", timer);
            }
            Console.WriteLine(timer);
        }
        cts.Cancel();

        Console.WriteLine("punkt testowy 7");
        var getAllAnswers =
            _context.UsersQuestionsAnswers
                .Where(obj => obj.Question.Equals(question.questionId))
                .OrderBy(obj=>obj.CreatedAt).Select(obj=> new
                {
                    obj.QuizSessionParticEntity.UserEntity.Username,
                    obj.QuizSessionParticEntity.Score
                }).ToList();
        Console.WriteLine("punkt testowy 8");
        await _hubUserContext.Clients.Group(token).SendAsync("QUESTION_RESULT_P2P", JsonSerializer.Serialize(getAllAnswers));
        Console.WriteLine("punkt testowy 9");
        quiz.CurrentQuestion++;
        _context.QuizLobbies.Update(quiz);
        await _context.SaveChangesAsync();
        Console.WriteLine("punkt testowy 10");
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