using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CollegeQuizWeb.Controllers;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Dto.SharedQuizes;
using CollegeQuizWeb.Entities;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CollegeQuizWeb.Services.SharedQuizesService;

public class SharedQuizesService : ISharedQuizesService
{
    private readonly ApplicationDbContext _context;
    public readonly static string ROOT_PATH = Directory.GetCurrentDirectory();
    public readonly static string FOLDER_PATH = $"{ROOT_PATH}/_Uploads/QuizImages";

    public SharedQuizesService(ApplicationDbContext context)
    {
        _context = context;
        
    }

    public async Task ShareQuizToken(ShareTokenPayloadDto obj)
    {
        SharedQuizesController controller = obj.ControllerReference;
        var token = _context.ShareTokensEntities.FirstOrDefault(t => t.Token.Equals(obj.Dto.ShareToken));
        string message = "";

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
            .FirstOrDefaultAsync(q =>q.SharedQuizesEntities.Any(p =>p.QuizId.Equals(id)));
        
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
                .Select(q => new
                {
                    qid=q.Id,
                    question = q.QuestionEntity.Name,
                    answer = q.Name,
                    goodAnswer = q.IsGood,
                    time_min = q.QuestionEntity.TimeMin,
                    time_sec = q.QuestionEntity.TimeSec,
                    type = q.QuestionEntity.QuestionType,
                })
                .GroupBy(q=>q.question)
                .Select(q=>new
                {
                    qid=q.Select(a=>a.qid).FirstOrDefault(),
                    question = q.Key,
                    answers = q.Select(a => a.answer).ToList(),
                    goodAnswers = q.Select(a => a.goodAnswer).ToList(),
                    time_min= q.Select(a=>a.time_min).FirstOrDefault(),
                    time_sec = q.Select(a=>a.time_sec).FirstOrDefault(),
                    type = q.Select(a=>a.type).FirstOrDefault(),
                })
                .OrderBy(q=>q.qid)
                .ToListAsync();

            controller.ViewBag.questions = questions;
                
            List<string> images = new();
            for (int i = 1; i <= questions.Count; i++)
            {
                images.Add(await GetQuestionImage(id,i));
            }
                
            controller.ViewBag.images = images;
        }
    }
    
    async Task<string> GetQuestionImage(long quizId, int qId)
    {
        string dir = $"{FOLDER_PATH}/{quizId}/question{qId}.jpg";
        if (!File.Exists(dir)) return "";
        Image image = Image.FromFile(dir);
        image = new Bitmap(image, new Size(500, 500));
        MemoryStream ms = new MemoryStream();
        image.Save(ms, ImageFormat.Jpeg);
        byte[] byteImg = ms.ToArray();
        string b64Img = Convert.ToBase64String(byteImg);
        ms.Close();

        return b64Img;
    }
}