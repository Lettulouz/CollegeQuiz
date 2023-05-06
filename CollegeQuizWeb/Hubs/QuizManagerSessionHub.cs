using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CollegeQuizWeb.API.Dto;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Dto.Quiz;
using CollegeQuizWeb.Entities;
using Microsoft.AspNetCore.Http;
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
        token = token.ToUpper();
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
        token = token.ToUpper();
        
        var quiz = await _context.QuizLobbies
            .Include(q => q.QuizEntity)
            .FirstOrDefaultAsync(q => q.Code.Equals(token));
        
        var questions = await _context.Answers
            .Include(q => q.QuestionEntity)
            .Where(q => q.QuestionEntity.QuizId.Equals(quiz!.QuizId))
            .GroupBy(q => q.QuestionEntity.Index)
            .Select(q => new
            {
                questionId = q.Key,
                question = q.First().QuestionEntity.Name,
                questionType = q.First().QuestionEntity.QuestionTypeEntity.TypeId,
                answers = q.Select(a => a.Name).ToList(),
                time_sec = q.Select(a => a.QuestionEntity.TimeMin * 60 + a.QuestionEntity.TimeSec).FirstOrDefault(),
                is_range = q.First().IsRange,
                step = q.First().Step,
                min = q.First().Min,
                max = q.First().Max,
                min_counted = q.First().MinCounted,
                max_counted = q.First().MaxCounted,
                image_url = GetImageUrl(q.Key, GetBasePath(), quiz!.QuizId),
            })
            .ToListAsync();
        
        await _hubUserContext.Clients.Group(token).SendAsync("START_GAME_P2P");
        if(quiz!.CurrentQuestion >= questions.Count) return;

        var question = questions[quiz.CurrentQuestion];

        int timer;
        await _hubUserContext.Clients.Group(token).SendAsync("QUESTION_P2P",  JsonSerializer.Serialize(question));
        var allAnswersWithGood = await _context.Answers.Include(t => t.QuestionEntity)
            .Where(t => t.QuestionEntity.Index.Equals(question.questionId) && t.QuestionEntity.QuizId.Equals(quiz.QuizId))
            .Select(q => new QuizLobbyAnswerData()
            {
                Text = q.Name,
                IsCorrect = q.IsGood
                
            }).ToListAsync();
        int correctRangeAnswer = 0;
        if (question.questionType == 5)
        {
            var rangeAnswer = await _context.Answers.Include(t => t.QuestionEntity)
                .FirstOrDefaultAsync(t => t.IsGood.Equals(true) && t.QuestionEntity.Index.Equals(question.questionId) &&
                                          t.QuestionEntity.QuizId.Equals(quiz.QuizId));
            correctRangeAnswer = rangeAnswer!.CorrectAnswer;
        }
        QuizLobbyQuestionData quizLobbyQuestionData = new()
        {
            QuestionName = question.question!,
            IsRange = question.is_range,
            Max = question.max,
            Min = question.min,
            MaxCounted = question.max_counted,
            MinCounted = question.min_counted,
            QuestionType = question.questionType,
            ImageUrl = question.image_url,
            TimeSec = question.time_sec,
            Step = question.step,
            QuestionId = quiz.CurrentQuestion + 1,
            Answers = allAnswersWithGood,
            CorrectAnswerRange = correctRangeAnswer
        };
        await Clients.Group(token).SendAsync("QUESTION_P2P",  JsonSerializer.Serialize(quizLobbyQuestionData));
        
        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken token2 = cts.Token;
        timer = question.time_sec;
        var periodicTimer= new PeriodicTimer(TimeSpan.FromSeconds(1));
        cts.CancelAfter(TimeSpan.FromSeconds(question.time_sec));
        
        LobbyQuestionTick questionTick = new LobbyQuestionTick() { Total = question.time_sec };
        try
        {
            while (!cts.IsCancellationRequested)
            {
                if (await periodicTimer.WaitForNextTickAsync(token2))
                {
                    timer--;
                    questionTick.Remaining = timer;
                    await _hubUserContext.Clients.Group(token).SendAsync("QUESTION_TIMER_P2P", JsonSerializer.Serialize(questionTick));
                    await Clients.Group(token).SendAsync("QUESTION_TIMER_P2P", JsonSerializer.Serialize(questionTick));
                }
                Console.WriteLine(timer);
                if (question.questionType != 3)
                {
                    var amountOfParticipants = _context.QuizSessionPartics.Include(q => q.QuizLobbyEntity)
                        .Where(x => x.QuizLobbyId.Equals(x.QuizLobbyEntity.Id) && x.QuizLobbyEntity.Code.Equals(token))
                        .Count();
                    var amountOfUniqueAnswers = _context.UsersQuestionsAnswers
                        .Where(x => x.QuizSessionParticEntity.QuizLobbyEntity.QuizId.Equals(quiz.QuizId) &&
                                    x.QuizSessionParticEntity.IsActive == true && x.Question.Equals(question.questionId))
                        .GroupBy(t => t.QuizSessionParticEntity.ParticipantId).Count();
                    if (amountOfUniqueAnswers >= amountOfParticipants)
                    {
                        timer = 0;
                        questionTick.Remaining = 0;
                        await Clients.Group(token).SendAsync("QUESTION_TIMER_P2P", JsonSerializer.Serialize(questionTick));
                    }
                }
                if (timer == 0)
                {
                    await Clients.Group(token).SendAsync("QUESTION_TIMER_P2P", JsonSerializer.Serialize(questionTick.Remaining=0));
                    cts.Cancel();
                }
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            cts.Dispose();
        }
        if (!question.is_range)
        {
            var currentAnswers = _context.Answers.Include(t => t.QuestionEntity)
                .Where(t => t.IsGood.Equals(true) && t.QuestionEntity.Index.Equals(question.questionId) &&
                            t.QuestionEntity.QuizId.Equals(quiz.QuizId))
                .Select(q => new
                {
                    AnswerName = q.Name
                }).ToList();
            
            var getAllAnswersForUpdate2 = _context.UsersQuestionsAnswers
                .Include(t => t.QuizSessionParticEntity)
                .ThenInclude(t => t.QuizLobbyEntity)
                .ThenInclude(t => t.UserEntity)
                .Where(t => t.Question.Equals(question.questionId) &&
                            t.QuizSessionParticEntity.QuizLobbyEntity.Code.Equals(token))
                .OrderBy(t => t.CreatedAt).ToList();
            
            IDictionary<string, long> corectAnswersNumber = new Dictionary<string, long>();
            foreach (var answer in getAllAnswersForUpdate2)
            {
                var currentAnswer = _context.Answers.Include(t => t.QuestionEntity)
                    .Where(t => t.IsGood.Equals(true) && t.QuestionEntity.Index.Equals(question.questionId) &&
                                t.QuestionEntity.QuizId.Equals(quiz.QuizId) && t.Name.Equals(question.answers[answer.Answer])
                    ).Count();
                
                if (currentAnswer > 0)
                {
                    if (corectAnswersNumber.ContainsKey(answer.QuizSessionParticEntity.ConnectionId))
                        corectAnswersNumber[answer.QuizSessionParticEntity.ConnectionId] += 1;
                    else
                        corectAnswersNumber.Add(answer.QuizSessionParticEntity.ConnectionId, 1);
                }
                else
                {
                    if (corectAnswersNumber.ContainsKey(answer.QuizSessionParticEntity.ConnectionId))
                        corectAnswersNumber[answer.QuizSessionParticEntity.ConnectionId] += currentAnswers.Count()+1;
                    else
                        corectAnswersNumber.Add(answer.QuizSessionParticEntity.ConnectionId, currentAnswers.Count()+1);
                    answer.QuizSessionParticEntity.CurrentStreak = 0;
                }
            }

            var getAllAnswersForUpdate = _context.UsersQuestionsAnswers
                .Include(t => t.QuizSessionParticEntity)
                .ThenInclude(t => t.QuizLobbyEntity)
                .ThenInclude(t => t.UserEntity)
                .Where(t => t.Question.Equals(question.questionId) &&
                            t.QuizSessionParticEntity.QuizLobbyEntity.Code.Equals(token))
                .GroupBy(t => t.ConnectionId)
                .Select(t => t.OrderByDescending(x=>x.CreatedAt).First())
                .ToList();

            IDictionary<string, long> newUserPoinst = new Dictionary<string, long>();
            var actuallTime = DateTime.Now;
            foreach (var answer in getAllAnswersForUpdate)
            {
                foreach (var currentAnswer in currentAnswers)
                {
                    if (question.answers[answer.Answer] == currentAnswer.AnswerName)
                    {
                        if (corectAnswersNumber[answer.QuizSessionParticEntity.ConnectionId] <=
                            currentAnswers.Count())
                        {
                            TimeSpan timeBetween = answer.CreatedAt - getAllAnswersForUpdate
                                .Where(t => question.answers[t.Answer] == currentAnswer.AnswerName)
                                .Min(t => t.CreatedAt);;
                            TimeSpan restOfTime = actuallTime - getAllAnswersForUpdate.Min(t => t.CreatedAt);
                            var wonPoints = Convert.ToInt64((1 - (timeBetween.TotalSeconds /
                                                                  restOfTime.TotalSeconds)) * 1000 *
                                                            (1 + answer.QuizSessionParticEntity.CurrentStreak * 0.02)*
                                                            (corectAnswersNumber[answer.QuizSessionParticEntity.ConnectionId]/Convert.ToDouble(currentAnswers.Count())));

                            newUserPoinst.Add(answer.QuizSessionParticEntity.ConnectionId, wonPoints);

                            answer.QuizSessionParticEntity.Score += wonPoints;
                        }

                        if (corectAnswersNumber[answer.QuizSessionParticEntity.ConnectionId] == currentAnswers.Count())
                            answer.QuizSessionParticEntity.CurrentStreak += 1;
                    }
                }
            }
            
            var notAnswered = _context.QuizSessionPartics
                .Where(qsp => !_context.UsersQuestionsAnswers
                    .Any(uqa => uqa.ConnectionId == qsp.Id && uqa.Question.Equals(question.questionId) && uqa.QuizSessionParticEntity.QuizLobbyEntity.Code.Equals(token)))
                .ToList();
            
            foreach (var user in notAnswered) user.CurrentStreak = 0;

            
            await _context.SaveChangesAsync();

            Console.WriteLine("punkt testowy 7");
            var allUsersPoints =
                _context.QuizSessionPartics
                    .Where(obj => obj.QuizLobbyEntity.Code.Equals(token) && obj.IsActive)
                    .Select(obj => new
                    {
                        obj.UserEntity.Username,
                        obj.Score,
                        isLast = (quiz.CurrentQuestion + 1 == questions.Count()),
                        newPoints = newUserPoinst.ContainsKey(obj.ConnectionId) ? newUserPoinst[obj.ConnectionId] : 0,
                        obj.CurrentStreak
                    }).OrderByDescending(obj => obj.Score);
            
            var topUsers = allUsersPoints.Take(5).ToList();
            
            var streakLeader =
                _context.QuizSessionPartics
                    .Where(obj => obj.QuizLobbyEntity.Code.Equals(token) && obj.IsActive)
                    .Select(obj => new
                    {
                        obj.UserEntity.Username,
                        obj.Score,
                        isLast = (quiz.CurrentQuestion + 1 == questions.Count()),
                        newPoints = newUserPoinst.ContainsKey(obj.ConnectionId) ? newUserPoinst[obj.ConnectionId] : 0,
                        obj.CurrentStreak
                    }).OrderByDescending(obj => obj.CurrentStreak).Take(1).ToList();
            var leaderboard = topUsers.Concat(streakLeader).ToList();

            Console.WriteLine("punkt testowy 8");

            List<ComputePoints> computePointsList = new List<ComputePoints>();
            foreach (var userPoints in allUsersPoints)
            {
                computePointsList.Add(new ComputePoints()
                {
                    Username = userPoints.Username,
                    Points = $"{userPoints.Score} (+ {userPoints.newPoints})",
                    IsGood = userPoints.newPoints != 0 ? "true" : "false"
                });
            }
            await Clients.Group(token).SendAsync("COMPUTE_ALL_POINTS_P2P", JsonSerializer.Serialize(computePointsList));
            
            await _hubUserContext.Clients.Group(token)
                .SendAsync("CORRECT_ANSWERS_SCREEN", JsonSerializer.Serialize(currentAnswers));
            Thread.Sleep(2000);

            await _hubUserContext.Clients.Group(token).SendAsync("QUESTION_RESULT_P2P", JsonSerializer.Serialize(leaderboard));
            await Clients.Group(token).SendAsync("QUESTION_RESULT_P2P", JsonSerializer.Serialize(leaderboard));
        }
        else
        {
            var currentAnswers = _context.Answers.Include(t => t.QuestionEntity)
                .Where(t => t.IsGood.Equals(true) && t.QuestionEntity.Index.Equals(question.questionId) &&
                            t.QuestionEntity.QuizId.Equals(quiz.QuizId))
                .Select(q => new
                {
                    AnswerName = q.Name,
                    AnswerCorrect = q.CorrectAnswer,
                    AnswerMin = q.Min,
                    AnswerMax = q.Max,
                    AnswerMinCounted = q.MinCounted,
                    AnswerMaxCounted = q.MaxCounted,
                    AnswerStep = q.Step
                }).ToList();

            var getAllAnswersForUpdate = _context.UsersQuestionsAnswers
                .Include(t => t.QuizSessionParticEntity)
                .ThenInclude(t => t.QuizLobbyEntity)
                .ThenInclude(t => t.UserEntity)
                .Where(t => t.Question.Equals(question.questionId) &&
                            t.QuizSessionParticEntity.QuizLobbyEntity.Code.Equals(token))
                .OrderBy(t => t.CreatedAt).ToList();
            
            IDictionary<string, long> newUserPoinst = new Dictionary<string, long>();
            var actuallTime = DateTime.Now;
            foreach (var answer in getAllAnswersForUpdate)
            {
                string[] minMax = answer.Range!.Split(',');
                int min=currentAnswers[0].AnswerMin, max=currentAnswers[0].AnswerMax;
                bool oneAnswer = false;
                if (minMax.Length.Equals(2))
                {
                    min = Int32.Parse(minMax[0]);
                    max = Int32.Parse(minMax[1]);
                }

                if (min.Equals(max))
                    oneAnswer = true;

                if (oneAnswer)
                {
                    if (min == currentAnswers[0].AnswerCorrect)
                    {
                        AddPoinstsCorrect(answer, newUserPoinst, getAllAnswersForUpdate,actuallTime,1.00, true, 750);
                    }
                    else
                    {
                        answer.QuizSessionParticEntity.CurrentStreak = 0;
                        if (newUserPoinst.ContainsKey(answer.QuizSessionParticEntity.ConnectionId))
                            newUserPoinst[answer.QuizSessionParticEntity.ConnectionId] += 0;
                        else
                            newUserPoinst.Add(answer.QuizSessionParticEntity.ConnectionId, 0);
                    }
                }
                else
                {
                    if (min >= currentAnswers[0].AnswerMinCounted && max <= currentAnswers[0].AnswerMaxCounted)
                    {
                        int amountOfNumbers = ((max - min) / currentAnswers[0].AnswerStep)+1;
                        double multiplier = (1.00 / amountOfNumbers*2) * 0.4;
                        multiplier = multiplier + 0.6;
                        AddPoinstsCorrect(answer, newUserPoinst, getAllAnswersForUpdate,actuallTime,multiplier, false, 400);
                    }
                    else
                    {
                        int amountOfNumbers = ((max - min) / currentAnswers[0].AnswerStep)+1;
                        int amountOfCorrectNumbers = ((currentAnswers[0].AnswerMax - currentAnswers[0].AnswerMin) 
                                               / currentAnswers[0].AnswerStep)+1;
                        int outsideLeft = currentAnswers[0].AnswerMinCounted - min;
                        int outsideRight = max - currentAnswers[0].AnswerMax;
                        int insideLeft = 0;
                        int insideRight = 0;
                        if (outsideLeft < 0) { insideLeft = -outsideLeft; outsideLeft = 0;}
                        if (outsideRight < 0){ insideRight = -outsideRight; outsideRight = 0;}
                        int totalCorrectInside = amountOfCorrectNumbers - insideLeft - insideRight;
                        if (totalCorrectInside < 0)
                            totalCorrectInside = 0;
                        double basicMultiplier = 0.2;
                        double rangeMultiplier = 1.00 / (amountOfNumbers*amountOfNumbers);
                        double correctMultiplier;
                        if (totalCorrectInside == 0)
                            correctMultiplier = 0.00;
                        else
                            correctMultiplier = totalCorrectInside / (double)amountOfCorrectNumbers;
                        double multiplier = (basicMultiplier + (basicMultiplier * rangeMultiplier)) * correctMultiplier;
                        AddPoinstsCorrect(answer, newUserPoinst, getAllAnswersForUpdate,actuallTime,multiplier, false, 0);
                    }
                }
            }
            await _context.SaveChangesAsync();

            Console.WriteLine("punkt testowy 7");
            var allUsersPoints =
                _context.QuizSessionPartics
                    .Where(obj => obj.QuizLobbyEntity.Code.Equals(token) && obj.IsActive)
                    .Select(obj => new
                    {
                        obj.UserEntity.Username,
                        obj.Score,
                        isLast = (quiz.CurrentQuestion + 1 == questions.Count()),
                        newPoints = newUserPoinst.ContainsKey(obj.ConnectionId) ? newUserPoinst[obj.ConnectionId] : 0,
                        obj.CurrentStreak
                    }).OrderByDescending(obj => obj.Score);

            var topUsers = allUsersPoints.Take(5).ToList();
            
            var streakLeader =
                _context.QuizSessionPartics
                    .Where(obj => obj.QuizLobbyEntity.Code.Equals(token) && obj.IsActive)
                    .Select(obj => new
                    {
                        obj.UserEntity.Username,
                        obj.Score,
                        isLast = (quiz.CurrentQuestion + 1 == questions.Count()),
                        newPoints = newUserPoinst.ContainsKey(obj.ConnectionId) ? newUserPoinst[obj.ConnectionId] : 0,
                        obj.CurrentStreak
                    }).OrderByDescending(obj => obj.CurrentStreak).Take(1).ToList();
            var leaderboard = topUsers.Concat(streakLeader).ToList();
            
            List<ComputePoints> computePointsList = new List<ComputePoints>();
            foreach (var userPoints in allUsersPoints)
            {
                computePointsList.Add(new ComputePoints()
                {
                    Username = userPoints.Username,
                    Points = $"{userPoints.Score} (+ {userPoints.newPoints})",
                    IsGood = userPoints.newPoints != 0 ? "true" : "false"
                });
            }
            await Clients.Group(token).SendAsync("COMPUTE_ALL_POINTS_P2P", JsonSerializer.Serialize(computePointsList));
            await _hubUserContext.Clients.Group(token).SendAsync("CORRECT_ANSWERS_SCREEN", JsonSerializer.Serialize(currentAnswers));
            
            Thread.Sleep(question.questionType == 5 ? 4000 : 2500);
            
            await _hubUserContext.Clients.Group(token).SendAsync("QUESTION_RESULT_P2P", JsonSerializer.Serialize(leaderboard));
            await Clients.Group(token).SendAsync("QUESTION_RESULT_P2P", JsonSerializer.Serialize(leaderboard));
        }
        quiz.CurrentQuestion++;
        
        _context.QuizLobbies.Update(quiz);
        await _context.SaveChangesAsync();
    }

    private void AddPoinstsCorrect(UsersQuestionsAnswersEntity answer, IDictionary<string,long> newUserPoinst,
        List<UsersQuestionsAnswersEntity> getAllAnswersForUpdate, DateTime actuallTime, double multiplier,
        bool continueStreak, int additionalPoints)
    {
        TimeSpan timeBetween = answer.CreatedAt - getAllAnswersForUpdate.Min(t => t.CreatedAt);
        TimeSpan restOfTime = actuallTime - getAllAnswersForUpdate.Min(t => t.CreatedAt);
        var wonPoints = Convert.ToInt64((1 - (timeBetween.TotalSeconds / restOfTime.TotalSeconds)) * 1000 *
                                        (1 + answer.QuizSessionParticEntity.CurrentStreak * 0.02) * multiplier+additionalPoints);
        if (newUserPoinst.ContainsKey(answer.QuizSessionParticEntity.ConnectionId))
            newUserPoinst[answer.QuizSessionParticEntity.ConnectionId] += wonPoints;
        else
            newUserPoinst.Add(answer.QuizSessionParticEntity.ConnectionId, wonPoints);
        answer.QuizSessionParticEntity.Score += wonPoints;
        if(continueStreak)
            answer.QuizSessionParticEntity.CurrentStreak += 1;
        else
            answer.QuizSessionParticEntity.CurrentStreak = 0;
    }
    
    public async override Task OnDisconnectedAsync(Exception? exception)
    {
        var hostUser = await _context.QuizLobbies
            .Include(q => q.UserEntity)
            .FirstOrDefaultAsync(q => q.HostConnId.Equals(Context.ConnectionId));
        if (hostUser == null) return;
        
        await _hubUserContext.Clients.Group(hostUser.Code).SendAsync("OnDisconnectedSession", "Host zakończył sesję.");

        var entities = _context.QuizSessionPartics
            .Include(q => q.QuizLobbyEntity)
            .Where(q => q.QuizLobbyEntity.Code.Equals(hostUser.Code));

        hostUser.IsEstabilished = false;
        hostUser.HostConnId = string.Empty;
        _context.QuizLobbies.Update(hostUser);
        
        if (entities.Count() > 0) _context.QuizSessionPartics.RemoveRange(entities);
        await _context.SaveChangesAsync();
    }
    
    private string GetBasePath()
    {
        string basePatch = "https://dominikpiskor.pl";
        HttpContext? context = Context.GetHttpContext();
        if (context != null)
        {
            basePatch = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.PathBase}";
        }
        return basePatch;
    }

    private static string GetImageUrl(int questionId, string basePatch, long quizId)
    {
        
        string fullPath = $"{Directory.GetCurrentDirectory()}/_Uploads/QuizImages/{quizId}/question{questionId}.jpg";
        string imageUrl = $"{basePatch}/api/v1/dotnet/quizapi/GetQuizImage/{quizId}/{questionId}";
        if (!File.Exists(fullPath))
        {
            imageUrl = string.Empty;
        }
        return imageUrl;
    }
}