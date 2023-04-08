using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Dto;
using CollegeQuizWeb.Dto.Quiz;
using CollegeQuizWeb.Entities;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace CollegeQuizWeb.Services.QuizService;

public class QuizService : IQuizService
{
    private readonly ApplicationDbContext _context;

    public QuizService(ApplicationDbContext context)
    {
        _context = context;
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
                Content = "Quiz o wybranej nazwie istnieje już na Twoim koncie. Wprowadź inną nazwę.",
            };
            controller.ViewBag.Alert = alertDto;
            return;
        }

        UserEntity? userEntity = await _context.Users.FirstOrDefaultAsync(u => u.Username.Equals(loggedUsername));
        QuizEntity quizEntity = new QuizEntity()
        {
            Name = dtoPayloader.Dto.QuizName,
            IsPublic = !dtoPayloader.Dto.IsPrivate,
            UserEntity = userEntity!
        };

        await _context.Quizes.AddAsync(quizEntity);
        await _context.SaveChangesAsync();

        AlertDto alertDto2 = new AlertDto()
        {
            Type = "alert-success",
            Content = $"Quiz o nazwie <strong>{dtoPayloader.Dto.QuizName}</strong> został pomyślnie utworzony."
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
    
    public async Task CreateQuizCode(QuizController controller, long quizId)
    {
        string username = controller.HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        //long userId = _context.Users.FirstOrDefaultAsync(u => u.Username.Equals(username)).Id;
        int tokenLife = 2; // in houres
        string generatedCode;
        bool isExactTheSame = false;
        
        do
        {
            generatedCode = Utilities.GenerateOtaToken(5, 2);
            var test = await _context.QuizLobbies.FirstOrDefaultAsync(c => c.Code.Equals(generatedCode));
            isExactTheSame = (test != null);
        } while (isExactTheSame);

        QuizLobbyEntity codeQuiz = new QuizLobbyEntity()
        {
            Code = generatedCode,
            ExpiredAt = DateTime.Now.AddHours(tokenLife),
            //UserHostId = 2,
            QuizId = quizId
        };
        await _context.AddAsync(codeQuiz);
        await _context.SaveChangesAsync();
        controller.ViewBag.Code = generatedCode;
    }
}