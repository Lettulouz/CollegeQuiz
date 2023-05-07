using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Dto;
using CollegeQuizWeb.Dto.Quiz;
using CollegeQuizWeb.Dto.SharedQuizes;
using CollegeQuizWeb.Entities;
using CollegeQuizWeb.Hubs;
using CollegeQuizWeb.Sftp;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using SkiaSharp;
using SKSvg = SkiaSharp.Extended.Svg.SKSvg;

namespace CollegeQuizWeb.Services.QuizService;

public class QuizService : IQuizService
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<QuizUserSessionHub> _hubContext;
    private readonly IAsyncSftpService _asyncSftpService;

    public QuizService(ApplicationDbContext context, IHubContext<QuizUserSessionHub> hubContext, IAsyncSftpService asyncSftpService)
    {
        _context = context;
        _hubContext = hubContext;
        _asyncSftpService = asyncSftpService;
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
            generatedToken = Utilities.GenerateOtaToken(12);
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
            .Where(x => x.QuizEntity.UserEntity.Username.Equals(userLogin) && !x.QuizEntity.IsHidden)
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
            .Where(x => x.UserId.Equals(userEntity!.Id) && !x.QuizEntity.IsHidden)
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
            .FirstOrDefaultAsync(q => q.Id == quizId && q.UserEntity.Username.Equals(userLogin) && !q.IsHidden);
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

        var quiz = await _context.Quizes.FirstOrDefaultAsync(q => q.Id.Equals(quizId) && !q.IsHidden);
        if (quiz == null) return true;

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
            
            await _hubContext.Clients.Group(test.Code).SendAsync("OnDisconnectedSession", Lang.HOST_DISCONECTED);
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

    public async Task<SKBitmap> GenerateQRCode(QuizController controller, string code)
    {
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCode = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);

        int moduleCount = qrCode.ModuleMatrix.Count;
        int qrSize = moduleCount * 40;
        int logoSize = (int)(qrSize * 0.175f);
        
        var bitmap = new SKBitmap(qrSize, qrSize);

        using (var surface = SKSurface.Create(new SKImageInfo(qrSize, qrSize)))
        {
            var canvas = surface.Canvas;
            
            canvas.Clear(SKColor.Parse("#f7fef5"));

            var paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.Black
            };

            // rysujemy kwadraty dla każdego modułu QR kodu
            for (int i = 0; i < moduleCount; i++)
            {
                for (int j = 0; j < moduleCount; j++)
                {
                    bool module = qrCode.ModuleMatrix[i][j];
                    if (module)
                    {
                        SKRect rect = SKRect.Create(j * 40, i * 40, 40, 40);
                        canvas.DrawRect(rect, paint);
                    }
                }
            }

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("https://quizazu.cdn.miloszgilga.pl/static/gfx/logo.svg");
            var content = await response.Content.ReadAsStringAsync();
            MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            
            // wczytujemy logo z pliku SVG i rysujemy na QR kodzie
            var svgDocument = new SKSvg(1, new SKSize(logoSize,logoSize));
            svgDocument.Load(memoryStream);
            float x = (qrSize - logoSize) / 2.0f;
            float y = (qrSize - logoSize) / 2.0f;
            canvas.DrawPicture(svgDocument.Picture, x, y);

            // tworzymy bitmapę na podstawie rysunku na płótnie
            bitmap = SKBitmap.Decode(surface.Snapshot().Encode());
            
            memoryStream.Close();
            httpClient.Dispose();
        }

        return bitmap;

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
        await _asyncSftpService.DeleteQuizImages(quizId);
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