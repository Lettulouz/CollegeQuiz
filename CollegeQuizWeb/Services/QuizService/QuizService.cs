using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Dto;
using CollegeQuizWeb.Dto.Quiz;
using CollegeQuizWeb.Entities;
using CollegeQuizWeb.Hubs;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using QRCoder;

namespace CollegeQuizWeb.Services.QuizService;

public class QuizService : IQuizService
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<QuizUserSessionHub> _hubContext;


    public QuizService(ApplicationDbContext context, IHubContext<QuizUserSessionHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }
    
    public async Task CreateNewQuiz(string loggedUsername, AddQuizDtoPayloader dtoPayloader)
    {
        QuizController controller = dtoPayloader.ControllerReference;
        if (!controller.ModelState.IsValid) return;
        
        var foundedQuizEntity = await _context.Quizes
            .Include(q => q.UserEntity)
            .FirstOrDefaultAsync(q => q.Name.Equals(dtoPayloader.Dto.QuizName, StringComparison.OrdinalIgnoreCase)
                                      && q.UserEntity.Username.Equals(loggedUsername));
        if (foundedQuizEntity != null)
        {
            AlertDto alertDto = new AlertDto()
            {
                Type = "alert-danger",
                Content = Lang.QUIZ_ALREADY_EXISTS,
            };
            controller.ViewBag.Alert = alertDto;
            return;
        }
        
        string generatedToken;
        bool isExactTheSame;
        do
        {
            generatedToken = Utilities.GenerateOtaToken(12, 1);
            var token = await _context.ShareTokensEntities.FirstOrDefaultAsync(t => t.Token.Equals(generatedToken));
            isExactTheSame = (token != null);
        } while (isExactTheSame);

        UserEntity? userEntity = await _context.Users.FirstOrDefaultAsync(u => u.Username.Equals(loggedUsername));
        QuizEntity quizEntity = new QuizEntity()
        {
            Name = dtoPayloader.Dto.QuizName,
            IsPublic = !dtoPayloader.Dto.IsPrivate,
            UserEntity = userEntity!
        };
        
        ShareTokensEntity shareTokensEntity = new ShareTokensEntity()
        {
            Token = generatedToken,
            QuizEntity = quizEntity
        };
        
        await _context.ShareTokensEntities.AddAsync(shareTokensEntity);
        await _context.Quizes.AddAsync(quizEntity);
        await _context.SaveChangesAsync();

        AlertDto alertDto2 = new AlertDto()
        {
            Type = "alert-success",
            Content = string.Format(Lang.QUIZ_ALREADY_EXISTS_NAME,dtoPayloader.Dto.QuizName)
        };
        controller.HttpContext.Session.SetString(SessionKey.MY_QUIZES_ALERT, JsonSerializer.Serialize(alertDto2));
        controller.Response.Redirect("/Quiz/MyQuizes");
    }

    public async Task<List<MyQuizDto>> GetMyQuizes(string userLogin)
    {
        return await _context.Quizes.Include(q => q.UserEntity)
            .Where(q => q.UserEntity.Username.Equals(userLogin))
            .Select(q => new MyQuizDto(){ Name = q.Name, Id = q.Id })
            .ToListAsync();
    }

    public async Task<QuizDetailsDto> GetQuizDetails(string userLogin, long quizId, QuizController controller)
    {
        var quizEntity = await _context.Quizes.Include(q => q.UserEntity)
            .FirstOrDefaultAsync(q => q.Id == quizId && q.UserEntity.Username.Equals(userLogin));
        if (quizEntity == null)
        {
            controller.Response.Redirect("/Quiz/MyQuizes");
            return new QuizDetailsDto();
        }
        return new QuizDetailsDto()
        {
            Name = quizEntity.Name
        };
    }
    
    public async Task CreateQuizCode(QuizController controller, string loggedUsername, long quizId)
    {
        long userId = _context.Users.FirstOrDefault(u => u.Username.Equals(loggedUsername))!.Id;
        var test = await _context.QuizLobbies
            .FirstOrDefaultAsync(q => q.UserHostId.Equals(userId) && q.QuizId.Equals(quizId));
        string generatedCode;
        do
        {
            generatedCode = Utilities.GenerateOtaToken(5, 2);
        } while (await _context.QuizLobbies.FirstOrDefaultAsync(c => c.Code.Equals(generatedCode)) != null);
        if (test != null)
        {
            var entities = await _context.QuizSessionPartics.Where(e => e.QuizLobbyId.Equals(test.Id)).ToListAsync();
            if (entities.Count() > 0) _context.QuizSessionPartics.RemoveRange(entities);
            
            await _hubContext.Clients.Group(test.Code).SendAsync("OnDisconectedSession", "Host zakończył sesję.");
            test.InGameScreen = "WAITING";
            test.Code = generatedCode;
            test.IsEstabilished = false;
            test.HostConnId = string.Empty;
            _context.QuizLobbies.Update(test);
            await _context.SaveChangesAsync();
            controller.ViewBag.Code = generatedCode;
            return;
        }
        QuizLobbyEntity codeQuiz = new QuizLobbyEntity()
        {
            Code = generatedCode,
            InGameScreen = "WAITING",
            UserHostId = userId,
            QuizId = quizId,
            HostConnId = string.Empty,
            IsEstabilished = false
        };
        await _context.QuizLobbies.AddAsync(codeQuiz);
        await _context.SaveChangesAsync();
        controller.ViewBag.Code = generatedCode;
    }

    public Bitmap GenerateQRCode(QuizController controller, string code)
    {
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);
        QRCode qrCode = new QRCode(qrCodeData);
        return qrCode.GetGraphic(50,Color.Black, Color.White,
            (Bitmap)Image.FromFile(@"wwwroot/qrCodeLogo.png"), 20, 1);
    }
}