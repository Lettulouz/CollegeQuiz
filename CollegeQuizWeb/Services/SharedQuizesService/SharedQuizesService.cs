using System.Linq;
using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Dto.SharedQuizes;
using CollegeQuizWeb.Entities;
using CollegeQuizWeb.Sftp;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CollegeQuizWeb.Services.SharedQuizesService;

public class SharedQuizesService : ISharedQuizesService
{
    private readonly ApplicationDbContext _context;
    private readonly IAsyncSftpService _asyncSftpService;
    
    public SharedQuizesService(ApplicationDbContext context, IAsyncSftpService asyncSftpService)
    {
        _context = context;
        _asyncSftpService = asyncSftpService;
    }

    public async Task ShareQuizToken(ShareTokenPayloadDto obj)
    {
        SharedQuizesController controller = obj.ControllerReference;
        var token = _context.ShareTokensEntities.FirstOrDefault(t => t.Token.Equals(obj.Dto.ShareToken));

        string? loggedUsername = controller.HttpContext.Session.GetString(SessionKey.IS_USER_LOGGED);
        UserEntity? userEntity = await _context.Users.FirstOrDefaultAsync(u => u.Username.Equals(loggedUsername));
        if (userEntity == null)
        {
            controller.HttpContext.Session.Remove(SessionKey.IS_USER_LOGGED);
            controller.Response.Redirect("/Auth/Login");
            return;
        }

        if (token == null)
        {
            controller.HttpContext.Session.SetString(SessionKey.QUIZ_CODE_MESSAGE_REDEEM,
                Lang.QUIZ_SHARED_TOKEN_BAD_ERROR);
            controller.HttpContext.Session.SetString(SessionKey.QUIZ_CODE_MESSAGE_REDEEM_TYPE, "alert-danger");
            controller.Response.Redirect("/SharedQuizes/Share");
            return;
        }

        var chechShareId =
            _context.ShareTokensEntities.Include(t => t.QuizEntity)
                .ThenInclude(u => u.UserEntity)
                .FirstOrDefault(u => u.QuizEntity.UserId.Equals(userEntity.Id) && u.Token.Equals(obj.Dto.ShareToken));

        var checkDuplicate =
            await _context.SharedQuizes.FirstOrDefaultAsync(s =>
                s.UserId.Equals(userEntity.Id) && s.QuizId.Equals(token.QuizId));


        if (chechShareId == null && checkDuplicate == null)
        {
            SharedQuizesEntity sharedQuizesEntity = new SharedQuizesEntity()
            {
                QuizId = token.QuizId,
                UserId = userEntity.Id
            };

            await _context.SharedQuizes.AddAsync(sharedQuizesEntity);
            await _context.SaveChangesAsync();
            controller.HttpContext.Session.SetString(SessionKey.QUIZ_CODE_MESSAGE_REDEEM,
                Lang.QUIZ_SHARED_TOKEN_SUCCESS);
            controller.HttpContext.Session.SetString(SessionKey.QUIZ_CODE_MESSAGE_REDEEM_TYPE, "alert-success");
            controller.Response.Redirect("/SharedQuizes/Share");
        }
        else
        {
            controller.HttpContext.Session.SetString(SessionKey.QUIZ_CODE_MESSAGE_REDEEM,
                Lang.QUIZ_SHARED_TOKEN_DUPLICATE_ERROR);
            controller.HttpContext.Session.SetString(SessionKey.QUIZ_CODE_MESSAGE_REDEEM_TYPE, "alert-danger");
            controller.Response.Redirect("/SharedQuizes/Share");
        }
    }

    public async Task ShareQuizInfo(long id, SharedQuizesController controller)
    {
        var quizShareInfo = await _context.Quizes
            .Include(s => s.SharedQuizesEntities)
            .FirstOrDefaultAsync(q =>q.SharedQuizesEntities.Any(p =>p.QuizId.Equals(id)) && !q.IsHidden);
        
        if (quizShareInfo == null)
        {
            controller.Response.Redirect("/Quiz/MyQuizes");
        }
        else
        {
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
    }
}