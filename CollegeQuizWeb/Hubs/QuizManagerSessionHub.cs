using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CollegeQuizWeb.API.Dto;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Dto.Quiz;
using CollegeQuizWeb.Entities;
using CollegeQuizWeb.Sftp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CollegeQuizWeb.Hubs;

public class QuizManagerSessionHub : Hub
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<QuizUserSessionHub> _hubUserContext;
    private readonly IAsyncSftpService _asyncSftpService;

    public QuizManagerSessionHub(ApplicationDbContext context, IHubContext<QuizUserSessionHub> hubUserContext,
        IAsyncSftpService asyncSftpService)
    {
        _context = context;
        _hubUserContext = hubUserContext;
        _asyncSftpService = asyncSftpService;
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
    
    public async Task START_GAME_P2P(string token)
    {
        token = token.ToUpper();
        
        var quiz = await _context.QuizLobbies
            .Include(q => q.QuizEntity)
            .FirstOrDefaultAsync(q => q.Code.Equals(token));
        
        var quizPreQuestions = await _context.Answers
            .Include(q => q.QuestionEntity)
            .Where(q => q.QuestionEntity.QuizId.Equals(quiz!.QuizId))
            .GroupBy(q => q.QuestionEntity.Index)
            .Select(q => new
            {
                QuestionId = q.Key,
                Question = q.First().QuestionEntity.Name,
                QuestionType = q.First().QuestionEntity.QuestionTypeEntity.TypeId,
                Answers = q.Select(a => a.Name).ToList(),
                TimeSec = q.Select(a => a.QuestionEntity.TimeMin * 60 + a.QuestionEntity.TimeSec).FirstOrDefault(),
                q.First().IsRange,
                q.First().Step,
                q.First().Min,
                q.First().Max,
                q.First().MinCounted,
                q.First().MaxCounted,
                ImageUrl = string.Empty,
                q.First().UpdatedAt,
            })
            .ToListAsync();
        
        await _hubUserContext.Clients.Group(token).SendAsync("START_GAME_P2P");

        if(quiz!.CurrentQuestion >= quizPreQuestions.Count) return;
        var questionPre = quizPreQuestions[quiz.CurrentQuestion];
        
        var question = new QuizManagerDto
        {
            QuestionId = questionPre.QuestionId,
            Question = questionPre.Question,
            QuestionType = questionPre.QuestionType,
            Answers = questionPre.Answers,
            TimeSec = questionPre.TimeSec,
            IsRange = questionPre.IsRange,
            Step = questionPre.Step,
            Min = questionPre.Min,
            Max = questionPre.Max,
            MinCounted = questionPre.MinCounted,
            MaxCounted = questionPre.MaxCounted,
            ImageUrl = string.Empty,
        };
        
        question.ImageUrl = await _asyncSftpService.GetImagePath(GetBasePath(), quiz.QuizId, question.QuestionId,
            questionPre.UpdatedAt);
        
        long timer;
        await _hubUserContext.Clients.Group(token).SendAsync("QUESTION_P2P",  JsonSerializer.Serialize(question));
        string tempVal = question.ImageUrl;
        
        string keyName = questionPre.UpdatedAt.ToString("yyyyMMddHHmmss");
        string imageUrlMobile = $"{GetBasePath()}/api/v1/dotnet/quizapi/GetQuizImage/{quiz.Id}/{question.QuestionId}/{keyName}";
        if (question.ImageUrl != string.Empty)
        {
            question.ImageUrl = imageUrlMobile;
        }
        else
        {
            question.ImageUrl = string.Empty;
        }
        await _hubUserContext.Clients.Group(token).SendAsync("QUESTION_MOBILE_P2P", JsonSerializer.Serialize(question));
        
        var allAnswersWithGood = await _context.Answers.Include(t => t.QuestionEntity)
            .Where(t => t.QuestionEntity.Index.Equals(question.QuestionId) && t.QuestionEntity.QuizId.Equals(quiz.QuizId))
            .Select(q => new QuizLobbyAnswerData()
            {
                Text = q.Name,
                IsCorrect = q.IsGood
                
            }).ToListAsync();
        int correctRangeAnswer = 0;
        if (question.QuestionType == 5)
        {
            var rangeAnswer = await _context.Answers.Include(t => t.QuestionEntity)
                .FirstOrDefaultAsync(t => t.IsGood.Equals(true) && t.QuestionEntity.Index.Equals(question.QuestionId) &&
                                          t.QuestionEntity.QuizId.Equals(quiz.QuizId));
            correctRangeAnswer = rangeAnswer!.CorrectAnswer;
        }
        QuizLobbyQuestionData quizLobbyQuestionData = new()
        {
            QuestionName = question.Question,
            IsRange = question.IsRange,
            Max = question.Max,
            Min = question.Min,
            MaxCounted = question.MaxCounted,
            MinCounted = question.MinCounted,
            QuestionType = question.QuestionType,
            ImageUrl = tempVal,
            TimeSec = question.TimeSec,
            Step = question.Step,
            QuestionId = quiz.CurrentQuestion + 1,
            Answers = allAnswersWithGood,
            CorrectAnswerRange = correctRangeAnswer
        };
        await Clients.Group(token).SendAsync("QUESTION_P2P", JsonSerializer.Serialize(quizLobbyQuestionData));

        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken token2 = cts.Token;
        timer = question.TimeSec;
        var periodicTimer= new PeriodicTimer(TimeSpan.FromSeconds(1));
        cts.CancelAfter(TimeSpan.FromSeconds(question.TimeSec));
        
        LobbyQuestionTick questionTick = new LobbyQuestionTick() { Total = question.TimeSec };
        try
        {
            while (!cts.IsCancellationRequested)
            {
                if (await periodicTimer.WaitForNextTickAsync(token2))
                {
                    timer--;
                    questionTick.Remaining = timer;
                    await _hubUserContext.Clients.Group(token)
                        .SendAsync("QUESTION_TIMER_P2P", JsonSerializer.Serialize(questionTick));
                    await Clients.Group(token).SendAsync("QUESTION_TIMER_P2P", JsonSerializer.Serialize(questionTick));
                }
                Console.WriteLine(timer);
                if (question.QuestionType != 3)
                {
                    var amountOfParticipants = _context.QuizSessionPartics.Include(q => q.QuizLobbyEntity)
                        .Where(x => x.QuizLobbyId.Equals(x.QuizLobbyEntity.Id) && x.QuizLobbyEntity.Code.Equals(token))
                        .Count();
                    var amountOfUniqueAnswers = _context.UsersQuestionsAnswers
                        .Where(x => x.QuizSessionParticEntity.QuizLobbyEntity.QuizId.Equals(quiz.QuizId) &&
                                    x.QuizSessionParticEntity.IsActive &&
                                    x.Question.Equals(question.QuestionId))
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
                    cts.Cancel();
                }
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            cts.Dispose();
        }

        questionTick.Remaining = 0;
        await Clients.Group(token).SendAsync("QUESTION_TIMER_P2P", JsonSerializer.Serialize(questionTick));
        
        if (!question.IsRange)
        {
            var currentAnswers = _context.Answers.Include(t => t.QuestionEntity)
                .Where(t => t.IsGood.Equals(true) && t.QuestionEntity.Index.Equals(question.QuestionId) &&
                            t.QuestionEntity.QuizId.Equals(quiz.QuizId))
                .Select(q => new
                {
                    AnswerName = q.Name
                }).ToList();
            
            var getAllAnswersForUpdate2 = _context.UsersQuestionsAnswers
                .Include(t => t.QuizSessionParticEntity)
                .ThenInclude(t => t.QuizLobbyEntity)
                .ThenInclude(t => t.UserEntity)
                .Where(t => t.Question.Equals(question.QuestionId) &&
                            t.QuizSessionParticEntity.QuizLobbyEntity.Code.Equals(token))
                .OrderBy(t => t.CreatedAt).ToList();
            
            IDictionary<string, long> corectAnswersNumber = new Dictionary<string, long>();
            foreach (var answer in getAllAnswersForUpdate2)
            {
                var currentAnswer = _context.Answers.Include(t => t.QuestionEntity)
                    .Where(t => t.IsGood.Equals(true) && t.QuestionEntity.Index.Equals(question.QuestionId) &&
                                t.QuestionEntity.QuizId.Equals(quiz.QuizId) && t.Name.Equals(question.Answers[answer.Answer])
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
                .Where(t => t.Question.Equals(question.QuestionId) &&
                            t.QuizSessionParticEntity.QuizLobbyEntity.Code.Equals(token))
                .GroupBy(t => t.ConnectionId)
                .Select(t => t.OrderByDescending(x=>x.CreatedAt).First())
                .ToList();
            
            DateTime bestTime = DateTime.Now;
            if (corectAnswersNumber.Any(kvp => kvp.Value > 0) && corectAnswersNumber.Any(kvp => kvp.Value <= currentAnswers.Count()))
            {
                bestTime = getAllAnswersForUpdate
                    .Where(t => 
                        corectAnswersNumber.ContainsKey(t.QuizSessionParticEntity.ConnectionId) &&
                        corectAnswersNumber[t.QuizSessionParticEntity.ConnectionId] > 0 &&
                        corectAnswersNumber[t.QuizSessionParticEntity.ConnectionId] <= currentAnswers.Count())
                    .Min(t => t.CreatedAt); 
            }

            IDictionary<string, long> newUserPoinst = new Dictionary<string, long>();
            var actuallTime = DateTime.Now;
            foreach (var answer in getAllAnswersForUpdate)
            {
                foreach (var currentAnswer in currentAnswers)
                {
                    if (question.Answers[answer.Answer] == currentAnswer.AnswerName)
                    {
                        if (corectAnswersNumber[answer.QuizSessionParticEntity.ConnectionId] <=
                            currentAnswers.Count())
                        {
                            TimeSpan timeBetween = answer.CreatedAt - bestTime;
                            TimeSpan restOfTime = actuallTime - getAllAnswersForUpdate.Min(t => t.CreatedAt);
                            var wonPoints = Convert.ToInt64((1 - (timeBetween.TotalSeconds / restOfTime.TotalSeconds)) * 1000 *
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
                    .Any(uqa => uqa.ConnectionId == qsp.Id && uqa.Question.Equals(question.QuestionId) &&
                                uqa.QuizSessionParticEntity.QuizLobbyEntity.Code.Equals(token)))
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
                        isLast = (quiz.CurrentQuestion + 1 == quizPreQuestions.Count()),
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
                        isLast = (quiz.CurrentQuestion + 1 == quizPreQuestions.Count()),
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
            
            await _hubUserContext.Clients.Group(token).SendAsync("CORRECT_ANSWERS_SCREEN", JsonSerializer.Serialize(currentAnswers));
            await Clients.Group(token).SendAsync("CORRECT_ANSWERS_SCREEN");
            Thread.Sleep(2000);

            await _hubUserContext.Clients.Group(token).SendAsync("QUESTION_RESULT_P2P", JsonSerializer.Serialize(leaderboard));
            await Clients.Group(token).SendAsync("QUESTION_RESULT_P2P", JsonSerializer.Serialize(leaderboard));
        }
        else
        {
            var currentAnswers = _context.Answers.Include(t => t.QuestionEntity)
                .Where(t => t.IsGood.Equals(true) && t.QuestionEntity.Index.Equals(question.QuestionId) &&
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
                .Where(t => t.Question.Equals(question.QuestionId) &&
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
                        int outsideLeft = (currentAnswers[0].AnswerMinCounted - min)/currentAnswers[0].AnswerStep;
                        int outsideRight = (max - currentAnswers[0].AnswerMax)/currentAnswers[0].AnswerStep;
                        int insideLeft = 0;
                        int insideRight = 0;
                        if (outsideLeft < 0) { insideLeft = -outsideLeft; }
                        if (outsideRight < 0) { insideRight = -outsideRight; }
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
                        if(max<currentAnswers[0].AnswerMinCounted || min > currentAnswers[0].AnswerMaxCounted)
                            correctMultiplier = 0.00;  
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
                        isLast = (quiz.CurrentQuestion + 1 == quizPreQuestions.Count()),
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
                        isLast = (quiz.CurrentQuestion + 1 == quizPreQuestions.Count()),
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
            await Clients.Group(token).SendAsync("CORRECT_ANSWERS_SCREEN");
            
            Thread.Sleep(question.QuestionType == 5 ? 4000 : 2500);
            
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
        hostUser.IsCreated = false;
        hostUser.HostConnId = string.Empty;
        _context.QuizLobbies.Update(hostUser);
        
        if (entities.Count() > 0) _context.QuizSessionPartics.RemoveRange(entities);
        await _context.SaveChangesAsync();
    }
}