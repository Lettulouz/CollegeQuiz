using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Dto.PublicQuizes;
using CollegeQuizWeb.Dto.Quiz;
using CollegeQuizWeb.Entities;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeQuizWeb.Services.PublicQuizesService;

public class PublicQuizesService : IPublicQuizesService
{
    private readonly ApplicationDbContext _context;
    
    public PublicQuizesService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MyQuizDto>> GetPublicQuizes()
    {
        return await _context.ShareTokensEntities.Include(t => t.QuizEntity)
            .Where(q => q.QuizEntity.IsPublic.Equals(true))
            .Select(q => new MyQuizDto()
                { Name = q.QuizEntity.Name, Id = q.QuizEntity.Id, Token = q.Token})
            .ToListAsync();
    }

    public async Task<List<MyQuizDto>> FilterQuizes(PublicDtoPayLoad obj)
    {
        PublicQuizesController controller = obj.ControllerReference;
        
        return await _context.ShareTokensEntities.Include(t => t.QuizEntity)
            .Where(q => q.QuizEntity.IsPublic.Equals(true) && q.QuizEntity.Name.Contains(obj.Dto.Name))
            .Select(q => new MyQuizDto()
                { Name = q.QuizEntity.Name, Id = q.QuizEntity.Id, Token = q.Token})
            .ToListAsync();
        
    }
    
    public async Task PublicQuizInfo(long id, PublicQuizesController controller)
    {
        var quizShareInfo = await _context.Quizes
            .Where(q => q.IsPublic.Equals(true) && q.Id.Equals(id))
            .FirstOrDefaultAsync();
        
        if (quizShareInfo == null)
        {
            controller.Response.Redirect("/PublicQuizes/Quizes");
        }
        else
        {
            controller.ViewBag.shareQuizInfo = quizShareInfo;
            
            controller.ViewBag.questions = await _context.Answers
                .Include(q => q.QuestionEntity)
                .Where(q => q.QuestionEntity.QuizId.Equals(quizShareInfo.Id))
                .Select(q => new
                {
                    question = q.QuestionEntity.Name,
                    answer = q.Name,
                    goodAnswer = q.IsGood,
                    time_min = q.QuestionEntity.TimeMin,
                    time_sec = q.QuestionEntity.TimeSec,
                })
                .GroupBy(q=>q.question)
                .Select(q=>new
                {
                    question = q.Key,
                    answers = q.Select(a => a.answer).ToList(),
                    goodAnswers = q.Select(a => a.goodAnswer).ToList(),
                    time_min= q.Sum(a=>a.time_min/4),
                    time_sec = q.Sum(a=>a.time_sec/4)
                })
                .ToListAsync();
        }
    }

    public async Task Share(string token, PublicQuizesController controller)
    {
        string? loggedUsername = controller.HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        var userEntity = await _context.Users.FirstOrDefaultAsync(u => u.Username.Equals(loggedUsername));
        if (userEntity == null)
        {
            controller.HttpContext.Session.Remove(SessionKey.IS_USER_LOGGED);
            controller.Response.Redirect("/Auth/Login");
            return;
        }
        var tokenHelper = await _context.ShareTokensEntities.FirstOrDefaultAsync(s => s.Token.Equals(token));
        var chechShareId =
            _context.ShareTokensEntities.Include(t => t.QuizEntity)
                .ThenInclude(u => u.UserEntity)
                .FirstOrDefault(u => u.QuizEntity.UserId.Equals(userEntity.Id) && u.Token.Equals(token));

        var checkDuplicate =
            await _context.SharedQuizes.FirstOrDefaultAsync(s =>
                s.UserId.Equals(userEntity.Id) && s.QuizId.Equals(tokenHelper.QuizId));


        if (chechShareId == null && checkDuplicate == null)
        {
            SharedQuizesEntity sharedQuizesEntity = new SharedQuizesEntity()
            {
                QuizId = tokenHelper.QuizId,
                UserId = userEntity.Id
            };

            await _context.SharedQuizes.AddAsync(sharedQuizesEntity);
            await _context.SaveChangesAsync();
            controller.HttpContext.Session.SetString(SessionKey.QUIZ_CODE_MESSAGE_REDEEM,
                Lang.QUIZ_SHARED_TOKEN_SUCCESS);
            controller.HttpContext.Session.SetString(SessionKey.QUIZ_CODE_MESSAGE_REDEEM_TYPE, "alert-success");
            controller.Response.Redirect("/PublicQuizes/Quizes");
        }
        else
        {
            controller.HttpContext.Session.SetString(SessionKey.QUIZ_CODE_MESSAGE_REDEEM,
                Lang.QUIZ_SHARED_TOKEN_DUPLICATE_ERROR);
            controller.HttpContext.Session.SetString(SessionKey.QUIZ_CODE_MESSAGE_REDEEM_TYPE, "alert-danger");
            controller.Response.Redirect("/PublicQuizes/Quizes");
        }
    }
}