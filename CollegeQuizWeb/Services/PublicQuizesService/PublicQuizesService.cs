using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Dto.PublicQuizes;
using CollegeQuizWeb.Dto.Quiz;
using CollegeQuizWeb.Entities;
using CollegeQuizWeb.Sftp;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CollegeQuizWeb.Services.PublicQuizesService;

public class PublicQuizesService : IPublicQuizesService
{
    private readonly ApplicationDbContext _context;
    private readonly IAsyncSftpService _asyncSftpService;
    
    public PublicQuizesService(ApplicationDbContext context, IAsyncSftpService asyncSftpService)
    {
        _context = context;
        _asyncSftpService = asyncSftpService;
    }

    public async Task<List<MyQuizDto>> GetPublicQuizes()
    {
        return await _context.ShareTokensEntities.Include(t => t.QuizEntity)
            .Where(q => q.QuizEntity.IsPublic.Equals(true) && !q.QuizEntity.IsHidden)
            .Select(q => new MyQuizDto()
                { Name = q.QuizEntity.Name,
                    Author = q.QuizEntity.UserEntity.Username ,
                    Id = q.QuizEntity.Id,
                    Token = q.Token,
                    CountOfQuestions = _context.Questions.Where(d => d.QuizId.Equals(q.QuizId)).Count()
                })
            .ToListAsync();
    }

    public async Task<List<MyQuizDto>> FilterQuizes(PublicDtoPayLoad obj)
    {
        return await _context.ShareTokensEntities.Include(t => t.QuizEntity)
            .Where(q => q.QuizEntity.IsPublic.Equals(true) && q.QuizEntity.Name.Contains(obj.Dto.Name)  && !q.QuizEntity.IsHidden)
            .Select(q => new MyQuizDto()
                { Name = q.QuizEntity.Name,
                    Author = q.QuizEntity.UserEntity.Username ,
                    Id = q.QuizEntity.Id,
                    Token = q.Token,
                    CountOfQuestions = _context.Questions.Where(d => d.QuizId.Equals(q.QuizId)).Count()
                })
            .ToListAsync();
        
    }

    public async Task Categories(PublicQuizesController controller)
    {
        controller.ViewBag.Categories = await _context.Categories.ToListAsync();
    }

    public async Task<List<SharedQuizesEntity>> Filter(PublicQuizesController controller, string[] categories)
    {
        controller.HttpContext.Session.SetString(SessionKey.CATEGORY_FILTER, "true");
        
        List<long> allCat = await _context.Categories
            .Select(c => c.Id)
            .ToListAsync();
        
        return await _context.SharedQuizes
            .Include(q => q.QuizEntity)
            .Include(q => q.UserEntity)
            .Include(q => q.QuizEntity.QuizCategoryEntities)
            .Where(q=>q.QuizEntity.QuizCategoryEntities
                .Any(qk=>allCat.Contains(qk.CategoryId)) && !q.QuizEntity.IsHidden)
                
            .ToListAsync();
    }
    
    public async Task PublicQuizInfo(long id, PublicQuizesController controller)
    {
        var quizShareInfo = await _context.Quizes
            .Where(q => q.IsPublic.Equals(true) && q.Id.Equals(id) && !q.IsHidden)
            .FirstOrDefaultAsync();
        
        if (quizShareInfo == null)
        {
            controller.Response.Redirect("/PublicQuizes/Quizes");
            return;
        }
        controller.ViewBag.shareQuizInfo = quizShareInfo;
            
        var questions = await _context.Answers
            .Include(q => q.QuestionEntity)
            .Where(q => q.QuestionEntity.QuizId.Equals(quizShareInfo.Id))
            .GroupBy(q=>q.QuestionEntity.Id)
            .Select(q => new
            {
                qid = q.Key,
                question = q.First().QuestionEntity.Name,
                type = q.First().QuestionEntity.QuestionType,
                answers = q.Select(a => a.Name).ToList(),
                goodAnswers = q.Select(a=>a.IsGood).ToList(),
                time_sec = q.First().QuestionEntity.TimeSec,
                time_min = q.First().QuestionEntity.TimeMin,
                step = q.First().Step,
                min = q.First().Min,
                max = q.First().Max,
                min_counted = q.First().MinCounted,
                max_counted = q.First().MaxCounted,
                correct_answer = q.First().CorrectAnswer,
                updated_at = q.First().UpdatedAt,
            })
            .ToListAsync();

        controller.ViewBag.questions = questions;
        controller.ViewBag.images = await _asyncSftpService
            .GetAllQuizImagesPath(Utilities.GetBaseUrl(controller), id, questions.Select(q => q.updated_at).ToList());
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
                s.UserId.Equals(userEntity.Id) && s.QuizId.Equals(tokenHelper!.QuizId));


        if (chechShareId == null && checkDuplicate == null)
        {
            SharedQuizesEntity sharedQuizesEntity = new SharedQuizesEntity()
            {
                QuizId = tokenHelper!.QuizId,
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