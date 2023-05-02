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
using CollegeQuizWeb.Dto.SharedQuizes;
using CollegeQuizWeb.Entities;
using CollegeQuizWeb.Hubs;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using Svg;

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
        return await _context.ShareTokensEntities.Include(t => t.QuizEntity)
            .ThenInclude(q =>q.UserEntity)
            .Where(x => x.QuizEntity.UserEntity.Username.Equals(userLogin))
            .Select(q => new MyQuizDto()
            {
                Name = q.QuizEntity.Name,
                Id = q.QuizEntity.Id,
                Token = q.Token,
                CountOfQuestions = _context.Questions.Where(d => d.QuizId.Equals(q.QuizId)).Count()
            })
            .ToListAsync();
    }
    
    public async Task<List<MyQuizSharedDto>> GetMyShareQuizes(string userLogin)
    {
        var userEntity = await _context.Users.FirstOrDefaultAsync(u => u.Username.Equals(userLogin));
        
        return await _context.SharedQuizes
            .Where(x => x.UserId.Equals(userEntity.Id))
            .Select(q => new MyQuizSharedDto()
            {
                Author = q.QuizEntity.UserEntity.Username,
                Name = q.QuizEntity.Name,
                Id = q.QuizEntity.Id,
                CountOfQuestions = _context.Questions.Where(d => d.QuizId.Equals(q.QuizId)).Count()
            })
            .ToListAsync();
    }

    public async Task<QuizDetailsDto> GetQuizDetails(string userLogin, long quizId, QuizController controller)
    {
        var quizEntity = await _context.Quizes
            .Include(q => q.UserEntity)
            .FirstOrDefaultAsync(q => q.Id == quizId && q.UserEntity.Username.Equals(userLogin));
        if (quizEntity == null)
        {
            controller.Response.Redirect("/Quiz/MyQuizes");
            return new QuizDetailsDto();
        }
        var quizLobby = await _context.QuizLobbies
            .FirstOrDefaultAsync(q => q.QuizId.Equals(quizId));
        if (quizLobby != null && quizLobby.IsEstabilished)
        {
            AlertDto alertDto2 = new AlertDto()
            {
                Type = "alert-danger",
                Content = Lang.DISABLE_EDITABLE_QUIZ
            };
            controller.HttpContext.Session.SetString(SessionKey.MY_QUIZES_ALERT, JsonSerializer.Serialize(alertDto2));
            controller.Response.Redirect("/Quiz/MyQuizes");
            return new QuizDetailsDto();
        }
        return new QuizDetailsDto()
        {
            Name = quizEntity.Name,
            Id = quizEntity.Id
        };
    }
    
    public async Task<bool> CreateQuizCode(QuizController controller, string loggedUsername, long quizId)
    {
        long userId = _context.Users.FirstOrDefault(u => u.Username.Equals(loggedUsername))!.Id;
        var test = await _context.QuizLobbies
            .Include(q => q.QuizEntity)
            .FirstOrDefaultAsync(q => q.UserHostId.Equals(userId) && q.QuizId.Equals(quizId));

        var quiz = await _context.Quizes.FirstOrDefaultAsync(q => q.Id.Equals(quizId));
        if (quiz == null) return false;
        
            int countOfQuestions = _context.Questions.Where(q => q.QuizId.Equals(quizId)).Count();
        if (countOfQuestions == 0) return true;
        
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
            test.InGameScreen = "WAITING_SCREEN";
            test.Code = generatedCode;
            test.IsEstabilished = false;
            test.HostConnId = string.Empty;
            _context.QuizLobbies.Update(test);
            await _context.SaveChangesAsync();
            controller.ViewBag.Code = generatedCode;
            controller.ViewBag.QuizName = quiz.Name;
            return false;
        }
        QuizLobbyEntity codeQuiz = new QuizLobbyEntity()
        {
            Code = generatedCode,
            InGameScreen = "WAITING_SCREEN",
            UserHostId = userId,
            QuizId = quizId,
            HostConnId = string.Empty,
            IsEstabilished = false
        };
        await _context.QuizLobbies.AddAsync(codeQuiz);
        await _context.SaveChangesAsync();
        controller.ViewBag.Code = generatedCode;
        controller.ViewBag.QuizName = quiz.Name;
        return false;
    }

    public Bitmap GenerateQRCode(QuizController controller, string code)
    {
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);
        QRCode qrCode = new QRCode(qrCodeData);
        var svgDocument = SvgDocument.Open(@"wwwroot/logo.svg");
        var bitmap = svgDocument.Draw();
        return qrCode.GetGraphic(50,Color.Black, Color.White,
            bitmap, 20, 1);
    }

    public async Task DeleteQuiz(long quizId, string loggedUsername, Controller controller)
    {
        string responseMessage, viewBagType;

        var deletedQuiz = await _context.Quizes
            .Include(q => q.UserEntity)
            .FirstOrDefaultAsync(q => q.UserEntity.Username.Equals(loggedUsername) && q.Id.Equals(quizId));
        var quizLobby = await _context.QuizLobbies
            .FirstOrDefaultAsync(q => q.QuizId.Equals(quizId));
        if (quizLobby != null && quizLobby.IsEstabilished)
        {
            responseMessage = Lang.DISABLE_REMOVABLE_QUIZ;
            viewBagType = "alert-danger";
        }
        else
        {
            if (deletedQuiz == null)
            {
                responseMessage = Lang.DELETE_QUIZ_NOT_FOUND;
                viewBagType = "alert-danger";
            }
            else
            {
                responseMessage = String.Format(Lang.SUCCESSFULL_DELETED_QUIZ, deletedQuiz.Name);
                viewBagType = "alert-success";
                _context.Quizes.Remove(deletedQuiz);
                await _context.SaveChangesAsync();
            }   
        }
        AlertDto alertDto2 = new AlertDto()
        {
            Type = viewBagType,
            Content = responseMessage
        };
        controller.HttpContext.Session.SetString(SessionKey.MY_QUIZES_ALERT, JsonSerializer.Serialize(alertDto2));
    }

    public async Task DeleteSharedQuiz(long quizId, string loggedUsername, Controller controller)
    {
        string responseMessage, viewBagType;
        
        var deletingSharedQuiz = await _context.SharedQuizes
            .Include(q => q.UserEntity)
            .Include(q => q.QuizEntity)
            .FirstOrDefaultAsync(q => q.UserEntity.Username.Equals(loggedUsername) && q.QuizId.Equals(quizId));
        if (deletingSharedQuiz == null)
        {
            responseMessage = Lang.DELETE_SHARED_QUIZ_NOT_FOUND;
            viewBagType = "alert-danger";
        }
        else
        {
            var quizLobby = await _context.QuizLobbies
                .FirstOrDefaultAsync(q => q.QuizId.Equals(quizId) && q.UserHostId.Equals(deletingSharedQuiz.UserEntity.Id));
            if (quizLobby != null && quizLobby.IsEstabilished)
            {
                responseMessage = Lang.DISABLE_DELETE_SHARED_QUIZ;
                viewBagType = "alert-danger";
            }
            else
            {
                responseMessage = String.Format(Lang.SUCCESSFULL_DELETED_SHARED_QUIZ, deletingSharedQuiz.QuizEntity.Name);
                viewBagType = "alert-success";
                _context.SharedQuizes.Remove(deletingSharedQuiz);
                await _context.SaveChangesAsync();
            }   
        }
        AlertDto alertDto2 = new AlertDto()
        {
            Type = viewBagType,
            Content = responseMessage
        };
        controller.HttpContext.Session.SetString(SessionKey.MY_QUIZES_ALERT, JsonSerializer.Serialize(alertDto2));
    }
}