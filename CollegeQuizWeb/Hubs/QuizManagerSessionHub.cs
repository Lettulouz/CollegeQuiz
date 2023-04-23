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
using Fluid.Ast.BinaryExpressions;
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
        try
        {
            while (!cts.IsCancellationRequested)
            {
                if (await periodicTimer.WaitForNextTickAsync(token2))
                {
                    timer--;
                    await _hubUserContext.Clients.Group(token).SendAsync("QUESTION_TIMER_P2P", timer);
                }
                Console.WriteLine(timer);
                if (timer == 0)
                {
                    cts.Cancel();
                }
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            cts.Dispose();
        }


        var currentAnswer = _context.Answers.Include(t => t.QuestionEntity)
                .Where(t => t.IsGood.Equals(true) && t.QuestionEntity.Index.Equals(question.questionId) &&
                t.QuestionEntity.QuizId.Equals(quiz.QuizId)).FirstOrDefault();

        var getAllAnswersForUpdate = _context.UsersQuestionsAnswers
            .Include(t => t.QuizSessionParticEntity)
            .ThenInclude(t => t.QuizLobbyEntity)
            .ThenInclude(t=>t.UserEntity)
            .Where(t => t.Question.Equals(question.questionId) &&
                          t.QuizSessionParticEntity.QuizLobbyEntity.Code.Equals(token))
            .OrderBy(t => t.CreatedAt).ToList();
        
        IDictionary<string, long> newUserPoinst = new Dictionary<string, long>();
        var actuallTime = DateTime.Now;
        foreach (var answer in getAllAnswersForUpdate)
        {
            if (question.answers[answer.Answer] == currentAnswer.Name)
            {
                TimeSpan timeBetween = answer.CreatedAt - getAllAnswersForUpdate.Min(t => t.CreatedAt);
                TimeSpan restOfTime = actuallTime - getAllAnswersForUpdate.Min(t => t.CreatedAt);
                var wonPoints = Convert.ToInt64((1 - (timeBetween.TotalSeconds /
                                                      restOfTime.TotalSeconds)) * 1000 *
                                                (1 + answer.QuizSessionParticEntity.CurrentStreak * 0.02));
                newUserPoinst.Add(answer.QuizSessionParticEntity.ConnectionId, wonPoints);
                answer.QuizSessionParticEntity.Score += wonPoints;
                answer.QuizSessionParticEntity.CurrentStreak += 1;
            }
            else
            {
                answer.QuizSessionParticEntity.CurrentStreak = 0;
                newUserPoinst.Add(answer.QuizSessionParticEntity.ConnectionId, 0);
            }
        }
        await _context.SaveChangesAsync();

        Console.WriteLine("punkt testowy 7");
       var topUsers =
           _context.QuizSessionPartics
               .Where(obj => obj.QuizLobbyEntity.Code.Equals(token))
               .Select(obj=> new
               {
                   obj.UserEntity.Username,
                   obj.Score,
                   isLast =  (quiz.CurrentQuestion + 1 == questions.Count()),
                   newPoints = newUserPoinst[obj.ConnectionId]
               }).OrderByDescending(obj=>obj.Score).Take(5).ToList();
       

       Console.WriteLine("punkt testowy 8");
       await _hubUserContext.Clients.Group(token).SendAsync("QUESTION_RESULT_P2P", JsonSerializer.Serialize(topUsers));
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