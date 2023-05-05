using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CollegeQuizWeb.API.Dto;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Entities;
using CollegeQuizWeb.Hubs;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CollegeQuizWeb.API.Services.QuizSession;

public class QuizSessionAPIService : IQuizSessionAPIService
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<QuizUserSessionHub> _hubUserContext;
    private readonly IHubContext<QuizManagerSessionHub> _hubManagerContext;

    public QuizSessionAPIService(ApplicationDbContext context, IHubContext<QuizUserSessionHub> hubUserContext,
        IHubContext<QuizManagerSessionHub> hubManagerContext)
    {
        
        _context = context;
        _hubUserContext = hubUserContext;
        _hubManagerContext = hubManagerContext;
    }

    public async Task<JoinToSessionDto> JoinRoom(string loggedUsername, string connectionId, string token)
    {
        token = token.ToUpper();
        var quizLobby = await _context.QuizLobbies
            .Include(l => l.QuizEntity)
            .FirstOrDefaultAsync(l => l.Code.Equals(token, StringComparison.InvariantCultureIgnoreCase) 
                                      && l.IsEstabilished == true);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username.Equals(loggedUsername));
        if (quizLobby == null) return new JoinToSessionDto()
        {
            IsGood = false,
            Message = String.Format(Lang.ERROR_TOKEN, token)
        };
        var alreadyJoinedInactive = await _context.QuizSessionPartics
            .Include(p => p.UserEntity)
            .Include(p => p.QuizLobbyEntity)
            .FirstOrDefaultAsync(p => p.UserEntity.Username.Equals(loggedUsername) &&
                                      p.QuizLobbyEntity.Code.Equals(token) && p.IsActive == false);

        var isHost = await _context.QuizLobbies.Include(l => l.UserEntity)
            .FirstOrDefaultAsync(l => l.UserEntity.Username.Equals(loggedUsername) && l.Code.Equals(token));
        if (isHost != null) return new JoinToSessionDto()
        {
            IsGood = false,
            Message = Lang.HOST_ERROR
        };

        ComputePoints computePoints = new ComputePoints() { IsGood = "none" };
        var alreadyExist = await _context.QuizSessionPartics
            .Include(u => u.UserEntity)
            .Include(u => u.QuizLobbyEntity)
            .FirstOrDefaultAsync(p => p.UserEntity.Username.Equals(loggedUsername) && 
                                      p.QuizLobbyId.Equals(quizLobby.Id) && p.IsActive == true);
        if (alreadyExist != null) return new JoinToSessionDto()
        {
            IsGood = false,
            Message = Lang.CURRNETLY_IN_GAME
        };
        if (alreadyJoinedInactive == null)
        {
            await _context.QuizSessionPartics.AddAsync(new QuizSessionParticEntity()
            {
                ConnectionId = connectionId,
                IsActive = true,
                QuizLobbyEntity = quizLobby,
                UserEntity = user!
            });
            computePoints.Username = user!.Username;
            computePoints.Points = "0 (+ 0)";
        }
        else
        {
            alreadyJoinedInactive.ConnectionId = connectionId;
            alreadyJoinedInactive.IsActive = true;
            computePoints.Username = alreadyJoinedInactive.UserEntity.Username;
            computePoints.Points = $"{alreadyJoinedInactive.Score} (+ 0)";
        }
        await _hubUserContext.Groups.AddToGroupAsync(connectionId, token);
        await _context.SaveChangesAsync();

        var restOfPartic = _context.QuizSessionPartics
            .Include(p => p.QuizLobbyEntity)
            .Include(p => p.UserEntity)
            .Where(p => p.QuizLobbyEntity.Code.Equals(token));
        
        await _hubManagerContext.Clients.Group(token).SendAsync("GetAllParticipants", JsonSerializer.Serialize(new
        {
            Connected = restOfPartic.Where(u => u.IsActive).Select(u => u.UserEntity.Username),
            Disconnected = restOfPartic.Where(u => !u.IsActive).Select(u => u.UserEntity.Username)
        }));

        await _hubManagerContext.Clients.Group(token)
            .SendAsync("USER_JOINABLE_POINTS_P2P", JsonSerializer.Serialize(computePoints));
        
        return new JoinToSessionDto()
        {
            IsGood = true,
            ScreenType = quizLobby.InGameScreen,
            QuizName = quizLobby.QuizEntity.Name
        };
    }

    public async Task<SimpleResponseDto> LeaveRoom(string loggedUsername, string connectionId, string token)
    {
        token = token.ToUpper();
        var quizSessionPart = await _context.QuizSessionPartics
            .Include(q => q.UserEntity)
            .Include(p => p.QuizLobbyEntity)
            .FirstOrDefaultAsync(p => p.UserEntity.Username.Equals(loggedUsername)
                                      && p.QuizLobbyEntity.Code.Equals(token));
        if (quizSessionPart == null) return new SimpleResponseDto()
        {
            IsGood = false,
            Message = Lang.CURRNETLY_NOT_IN_GAME
        };
        quizSessionPart.IsActive = false;
        await _context.SaveChangesAsync();
        await _hubUserContext.Groups.RemoveFromGroupAsync(connectionId, token);

        var restOfPartic = _context.QuizSessionPartics
            .Include(p => p.QuizLobbyEntity)
            .Include(p => p.UserEntity)
            .Where(p => p.QuizLobbyEntity.Code.Equals(token));
        
        await _hubManagerContext.Clients.Group(token).SendAsync("GetAllParticipants", JsonSerializer.Serialize(new
        {
            Connected = restOfPartic.Where(u => u.IsActive).Select(u => u.UserEntity.Username),
            Disconnected = restOfPartic.Where(u => !u.IsActive).Select(u => u.UserEntity.Username)
        }));
        
        await _hubManagerContext.Clients.Group(token).SendAsync("USER_ON_LEAVE_ROOM_P2P", loggedUsername);
        return new SimpleResponseDto()
        {
            IsGood = true,
            Message = Lang.SESSION_LEFT_ENTER_TOKEN
        };
    }

    public async Task<JoinToSessionDto> EstabilishedHostRoom(string loggedUsername, string connectionId, string token)
    {
        token = token.ToUpper();
        var isHost = await _context.QuizLobbies
            .Include(p => p.UserEntity)
            .FirstOrDefaultAsync(p => p.UserEntity.Username.Equals(loggedUsername) && p.Code.Equals(token));
        if (isHost == null) return new JoinToSessionDto()
        {
            IsGood = false,
            Message = Lang.HOST_NOT_FOUND
        };
        isHost.IsEstabilished = true;
        isHost.InGameScreen = "WAITING_SCREEN";
        isHost.HostConnId = connectionId;
        isHost.CurrentQuestion = 0;
        _context.QuizLobbies.Update(isHost);
        await _context.SaveChangesAsync();
        await _hubManagerContext.Groups.AddToGroupAsync(connectionId, token);
        return new JoinToSessionDto()
        {
            IsGood = true,
        };
    }

    public async Task<QuizLobbyInfoDto> GetLobbyData(string loggedUsername, string token)
    {
        token = token.ToUpper();
        var lobbyData = await _context.QuizLobbies
            .Include(l => l.QuizEntity)
            .Include(l => l.UserEntity)
            .FirstOrDefaultAsync(l => l.Code.Equals(token));
        
        if (lobbyData == null) return new QuizLobbyInfoDto();

        int countOfQuestions = _context.Questions.Where(q => q.QuizId.Equals(lobbyData.QuizId)).Count();
        return new QuizLobbyInfoDto()
        {
            Name = lobbyData.QuizEntity.Name,
            Host = lobbyData.UserEntity.Username,
            QuestionsCount = countOfQuestions
        };
    }

    public async Task SendAnswer(string connectionId, string questionId, string answerId, bool isMultiAnswer)
    {
        int questionNum, answerNum;
        if (answerId.Length.Equals(0)) return;
        
        var token = await _context.QuizSessionPartics
            .Include(q => q.QuizLobbyEntity)
            .FirstOrDefaultAsync(q => q.ConnectionId.Equals(connectionId));
        if (token == null) return;
        
        if (answerId[0].Equals('r'))
        {
            if (!Int32.TryParse(questionId, out questionNum)) return;
            string answerRange = answerId.TrimStart('r');
            var connetionIdInDb = await _context.QuizSessionPartics
                .Include(q => q.UserEntity)
                .FirstOrDefaultAsync(obj => obj.ConnectionId.Equals(connectionId));
        
            if (connetionIdInDb == null) return;
        
            var answerInDb = await _context.UsersQuestionsAnswers
                    .Include(p => p.QuizSessionParticEntity)
                    .Where(p =>
                        p.Question.Equals(questionNum) && p.QuizSessionParticEntity.ConnectionId.Equals(connectionId))
                    .ToListAsync();
        
            if (!answerInDb.Count.Equals(0)) return;
        
            UsersQuestionsAnswersEntity usersAnswersEntity = new();
            usersAnswersEntity.ConnectionId = connetionIdInDb.Id;
            usersAnswersEntity.Question = questionNum;
            usersAnswersEntity.Range = answerRange;
        
            await _context.UsersQuestionsAnswers.AddAsync(usersAnswersEntity);
            
            CurrentGameStatusQuestions currentGameStatusQuestions = new()
            {
                Username = connetionIdInDb.UserEntity.Username,
                SelectedAnswer = answerRange.Replace(",", " -> "),
            };
            await _hubManagerContext.Clients.Group(token.QuizLobbyEntity.Code)
                .SendAsync("USER_SELECT_ANSWER_P2P", JsonSerializer.Serialize(currentGameStatusQuestions));
        }
        else
        {
            if (!Int32.TryParse(questionId, out questionNum) || !Int32.TryParse(answerId, out answerNum))
                return;
            var connetionIdInDb = await _context.QuizSessionPartics
                .Include(q => q.UserEntity)
                .FirstOrDefaultAsync(obj => obj.ConnectionId.Equals(connectionId));
        
            if (connetionIdInDb == null) return;
        
            if (isMultiAnswer)
            {
                var answerInDb = await _context.UsersQuestionsAnswers
                    .Include(p => p.QuizSessionParticEntity)
                    .Where(p => p.Question.Equals(questionNum)
                                && p.QuizSessionParticEntity.ConnectionId.Equals(connectionId)
                                && p.Answer.Equals(answerId))
                    .ToListAsync();
                
                if (!answerInDb.Count.Equals(0)) return;
            }
            else
            {
                var answerInDb = await _context.UsersQuestionsAnswers
                    .Include(p => p.QuizSessionParticEntity)
                    .Where(p => p.Question.Equals(questionNum) && p.QuizSessionParticEntity.ConnectionId.Equals(connectionId))
                    .ToListAsync();
                
                if (!answerInDb.Count.Equals(0)) return;
            }
            UsersQuestionsAnswersEntity usersAnswersEntity = new();
            usersAnswersEntity.ConnectionId = connetionIdInDb.Id;
            usersAnswersEntity.Question = questionNum;
            usersAnswersEntity.Answer = answerNum;
        
            await _context.UsersQuestionsAnswers.AddAsync(usersAnswersEntity);
            
            CurrentGameStatusQuestions currentGameStatusQuestions = new()
            {
                Username = connetionIdInDb.UserEntity.Username,
                SelectedAnswer = $"{answerNum}",
            };
            await _hubManagerContext.Clients.Group(token.QuizLobbyEntity.Code)
                .SendAsync("USER_SELECT_ANSWER_P2P", JsonSerializer.Serialize(currentGameStatusQuestions));
        }
        await _context.SaveChangesAsync();
    }

    public async Task RemoveFromSession(string loggedUsername, string token, string username)
    {
        var particToDelete = _context.QuizSessionPartics
            .Include(x=>x.QuizLobbyEntity)
            .Where(x => x.QuizLobbyEntity.Code.Equals(token))
            .FirstOrDefault(x=> x.UserEntity.Username.Equals(username));
        if (particToDelete != null)
        {
            _context.QuizSessionPartics.Remove(particToDelete);
            await _context.SaveChangesAsync();
            var restOfPartic = _context.QuizSessionPartics
                .Include(p => p.QuizLobbyEntity)
                .Include(p => p.UserEntity)
                .Where(p => p.QuizLobbyEntity.Code.Equals(token));
        
            await _hubManagerContext.Clients.Group(token).SendAsync("GetAllParticipants", JsonSerializer.Serialize(new
            {
                Connected = restOfPartic.Where(u => u.IsActive).Select(u => u.UserEntity.Username),
                Disconnected = restOfPartic.Where(u => !u.IsActive).Select(u => u.UserEntity.Username)
            }));
            await _hubUserContext.Clients.Client(particToDelete.ConnectionId)
                .SendAsync("OnDisconnectedSession", Lang.HOST_DISCONECT_USER);
        }
    }
}