using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CollegeQuizWeb.API.Dto;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Dto.Quiz;
using CollegeQuizWeb.Entities;
using CollegeQuizWeb.Hubs;
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
        var quizLobby = await _context.QuizLobbies
            .Include(l => l.QuizEntity)
            .FirstOrDefaultAsync(l => l.Code.Equals(token, StringComparison.InvariantCultureIgnoreCase) 
                                      && l.IsEstabilished == true);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username.Equals(loggedUsername));
        if (quizLobby == null) return new JoinToSessionDto()
        {
            IsGood = false,
            Message = $"Podany przez Ciebie kod <strong>{token}</strong> nie istnieje lub uległ przedawnieniu."
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
            Message = $"Host gry nie może jednocześnie być hostem i brać w niej udziału."
        };

        var alreadyExist = await _context.QuizSessionPartics
            .Include(u => u.UserEntity)
            .Include(u => u.QuizLobbyEntity)
            .FirstOrDefaultAsync(p => p.UserEntity.Username.Equals(loggedUsername) && 
                                      p.QuizLobbyId.Equals(quizLobby.Id) && p.IsActive == true);
        if (alreadyExist != null) return new JoinToSessionDto()
        {
            IsGood = false,
            Message = $"Na tym koncie obecnie prowadzona jest rozgrywka w sesji. Użyj innego konta."
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
        }
        else
        {
            alreadyJoinedInactive.ConnectionId = connectionId;
            alreadyJoinedInactive.IsActive = true;
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
        
        return new JoinToSessionDto()
        {
            IsGood = true,
            ScreenType = quizLobby.InGameScreen,
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
        return new SimpleResponseDto()
        {
            IsGood = true,
            Message = "Wyszedłeś z sesji, aby wejść ponownie wprowadź kod quizu"
        };
    }

    public async Task<JoinToSessionDto> EstabilishedHostRoom(string loggedUsername, string connectionId, string token)
    {
        var isHost = await _context.QuizLobbies
            .Include(p => p.UserEntity)
            .FirstOrDefaultAsync(p => p.UserEntity.Username.Equals(loggedUsername) && p.Code.Equals(token));
        if (isHost == null) return new JoinToSessionDto()
        {
            IsGood = false,
            Message = "Nie znaleziono aktywnego hosta sesji gry."
        };
        isHost.IsEstabilished = true;
        isHost.InGameScreen = "WAITING_SCREEN";
        isHost.HostConnId = connectionId;
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
        var lobbyData = await _context.QuizLobbies
            .Include(l => l.QuizEntity)
            .Include(l => l.UserEntity)
            .FirstOrDefaultAsync(l => l.Code.Equals(token));
        if (lobbyData == null) return new QuizLobbyInfoDto();
        return new QuizLobbyInfoDto()
        {
            Name = lobbyData.QuizEntity.Name,
            Host = lobbyData.UserEntity.Username
        };
    }

    public async Task SendAnswer(string connectionId, string token, string questionId, string answerId)
    {
        int questionNum, answerNum;
        if (!Int32.TryParse(questionId, out questionNum) || !Int32.TryParse(answerId, out answerNum))
            return;

        var connetionIdInDb = _context.QuizSessionPartics
            .FirstOrDefault(obj => obj.ConnectionId.Equals(connectionId));

        if (connetionIdInDb == null)
            return;

        var answerInDb =
            _context.UsersQuestionsAnswers
                .Include(p => p.QuizSessionParticEntity)
                .Where(p =>
                    p.Question.Equals(questionNum) && p.QuizSessionParticEntity.ConnectionId.Equals(connectionId))
                .ToList();

        if (!answerInDb.Count.Equals(0))
            return;
        
        UsersQuestionsAnswersEntity usersAnswersEntity = new();
        usersAnswersEntity.ConnectionId = connetionIdInDb.Id;
        usersAnswersEntity.Question = questionNum;
        usersAnswersEntity.Answer = answerNum;

        _context.Add(usersAnswersEntity);
        await _context.SaveChangesAsync();
    }
}