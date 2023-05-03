using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CollegeQuizWeb.API.Dto;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Entities;
using CollegeQuizWeb.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeQuizWeb.API.Services.Quiz;

public class QuizAPIService : IQuizAPIService
{
    private readonly ApplicationDbContext _context;
    
    public readonly static string[] ACCEPTABLE_IMAGE_TYPES = { "image/jpeg", "image/png", "image/jpg" };
    public readonly static string ROOT_PATH = Directory.GetCurrentDirectory();
    public readonly static string FOLDER_PATH = $"{ROOT_PATH}/_Uploads/QuizImages";
    
    public QuizAPIService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<SimpleResponseDto> AddQuizQuestions(long quizId, AggregateQuestionsReqDto dto, string loggedUsername)
    {
        var quizEntity = await _context.Quizes.Include(q => q.UserEntity)
            .FirstOrDefaultAsync(q => q.Id == quizId && q.UserEntity.Username.Equals(loggedUsername));
        if (quizEntity == null) return new SimpleResponseDto()
        {
            IsGood = false,
            Message = Lang.QUIZ_NOT_FOUND
        };
        // usunięcie poprzednich pytań
        var prevQuestions = _context.Questions.Where(q => q.QuizId.Equals(quizEntity.Id));
        _context.Questions.RemoveRange(prevQuestions);

        // sprawdzenie, czy odpowiedzi nie są takie same
        var flattedQuestions = dto.Aggregate.Select(a => a.Text).ToList();
        if (flattedQuestions.Count != flattedQuestions.Distinct().Count()) return new SimpleResponseDto()
        {
            IsGood = false,
            Message = Lang.QUESTIONS_REPEATED
        };
        foreach (var question in dto.Aggregate)
        {
            if (!QuizModes.CheckIfUserHasPermissions(question.Type, quizEntity.UserEntity.AccountStatus))
            {
                return new SimpleResponseDto()
                {
                    IsGood = false,
                    Message = String.Format(Lang.PACKAGE_DOES_NOT_ALLOW, QuizModes.GetValueFromSlug(question.Type))
                };
            }
            int min, sec;
            if (!int.TryParse(question.TimeMin, out min) || !int.TryParse(question.TimeSec, out sec)) 
            {
                return new SimpleResponseDto()
                {
                    IsGood = false,
                    Message = Lang.GIVEN_VALUES_NOT_NUMBERS
                };
            }
            if (sec < 5 || sec > 59) return new SimpleResponseDto()
            {
                IsGood = false,
                Message = Lang.WRONG_TIME_INTERVALS
            };
            if (min < 0) min = 0;
            var questionType = await _context.QuestionTypeEntities.FirstOrDefaultAsync(t => t.Name.Equals(question.Type));
            if (questionType == null) return new SimpleResponseDto()
            {
                IsGood = false,
                Message = Lang.WRONG_QUESTION_TYPE
            };
            QuestionEntity questionEntity = new QuestionEntity()
            {
                Index = question.Id,
                Name = question.Text,
                TimeMin = min,
                TimeSec = sec,
                QuizEntity = quizEntity,
                QuestionTypeEntity = questionType
            };
            // sprawdzenie, czy odpowiedzi nie są takie same
            var flattedAnswers = question.Answers.Select(a => a.Text).ToList();
            if (flattedAnswers.Count != flattedAnswers.Distinct().Count()) return new SimpleResponseDto()
            {
                IsGood = false,
                Message = String.Format(Lang.QUESTION_REPEATED, question.Text)
            };
            foreach (var answer in question.Answers)
            {
                AnswerEntity answerEntity;
                if (question.Type.Equals("RANGE"))
                {
                    answerEntity = new AnswerEntity()
                    {
                        Name = "RANGE",
                        IsGood = answer.IsCorrect,
                        IsRange = true,
                        Min = answer.Min,
                        Max = answer.Max,
                        MinCounted = answer.MinCounted,
                        MaxCounted = answer.MaxCounted,
                        Step = answer.Step,
                        QuestionEntity = questionEntity,
                        CorrectAnswer = answer.CorrectAns,
                    };
                }
                else
                {
                    answerEntity = new AnswerEntity()
                    {
                        Name = answer.Text,
                        IsGood = answer.IsCorrect,
                        IsRange = false,
                        QuestionEntity = questionEntity
                    };
                }
                await _context.Answers.AddAsync(answerEntity);
            }
            await _context.Questions.AddAsync(questionEntity);
        }
        await _context.SaveChangesAsync();
        return new SimpleResponseDto()
        {
            IsGood = true,
            Message = String.Format(Lang.QUIZ_NAME_UPDATED, quizEntity.Name)
        };
    }

    public async Task<AggregateQuestionsReqDto> GetQuizQuestions(long quizId, string loggedUsername, Controller controller)
    {
        var quizEntity = await _context.Quizes
            .Include(q => q.UserEntity)
            .FirstOrDefaultAsync(q => q.Id == quizId && q.UserEntity.Username.Equals(loggedUsername));
        if (quizEntity == null)
        {
            return new AggregateQuestionsReqDto()
            {
                Aggregate = new List<QuizQuestionsReqDto>()
            };
        }
        AggregateQuestionsReqDto dto = new AggregateQuestionsReqDto();
        dto.Aggregate = new List<QuizQuestionsReqDto>();
        
        var questions = _context.Questions
            .Include(q => q.QuestionTypeEntity)
            .Where(q => q.QuizId == quizEntity.Id);
        foreach (var question in questions)
        {
            var answers = _context.Answers.Where(a => a.QuestionId == question.Id);
            List<AnswerDto> answerDtos = new List<AnswerDto>();
            int idx = 0;
            foreach (var answer in answers)
            {
                AnswerDto answerDto;
                if (answer.IsRange)
                {
                    answerDto = new AnswerDto()
                    {
                        Id = ++idx,
                        Text = answer.Name,
                        IsCorrect = answer.IsGood,
                        IsRange = answer.IsRange,
                        Min = answer.Min,
                        Max = answer.Max,
                        MinCounted = answer.MinCounted,
                        MaxCounted = answer.MaxCounted,
                        Step = answer.Step,
                        CorrectAns = answer.CorrectAnswer,
                    };
                }
                else
                {
                    answerDto = new AnswerDto()
                    {
                        Id = ++idx,
                        Text = answer.Name,
                        IsCorrect = answer.IsGood,
                        IsRange = answer.IsRange
                    };
                }
                answerDtos.Add(answerDto);
            }
            string imageUrl =
                $"{Utilities.GetBaseUrl(controller)}/api/v1/dotnet/quizapi/GetQuizImage/{quizId}/{question.Index}";
            string fullPath = $"{FOLDER_PATH}/{quizId}/question{question.Index}.jpg";
            if (!File.Exists(fullPath))
            {
                imageUrl = string.Empty;
            }
            QuizQuestionsReqDto questionsReqDto = new QuizQuestionsReqDto()
            {
                Id = question.Index,
                Text = question.Name,
                TimeMin = question.TimeMin.ToString(),
                TimeSec = question.TimeSec.ToString(),
                Type = question.QuestionTypeEntity.Name,
                Answers = answerDtos,
                ImageUrl = imageUrl,
            };
            dto.Aggregate.Add(questionsReqDto);
        }
        dto.AvailableModes = QuizModes.GetModesForUserPacket(quizEntity.UserEntity.AccountStatus);
        dto.PermissionModesMessage = QuizModes.GetPermissionsMessage(quizEntity.UserEntity.AccountStatus);
        return dto;
    }

    public async Task<SimpleResponseDto> UpdateQuizName(long quizId, string newQuizName, string loggedUsername)
    {
        var quizEntity = await _context.Quizes.Include(q => q.UserEntity)
            .FirstOrDefaultAsync(q => q.Id == quizId && q.UserEntity.Username.Equals(loggedUsername));
        if (quizEntity == null) return new SimpleResponseDto()
        {
            IsGood = false,
            Message = Lang.QUIZ_NOT_FOUND
        };
        int countOfSameName = _context.Quizes.Include(q => q.UserEntity)
            .Where(q => q.Name.Equals(newQuizName, StringComparison.OrdinalIgnoreCase)
                        && q.UserEntity.Id.Equals(quizEntity.UserEntity.Id) && q.Id != quizId)
            .Count();
        if (countOfSameName != 0) return new SimpleResponseDto()
        {
            IsGood = false,
            Message = String.Format(Lang.QUIZ_NAME_REPEATED, newQuizName)
        };
        string prevName = quizEntity.Name;
        quizEntity.Name = newQuizName;
        _context.Quizes.Update(quizEntity);
        await _context.SaveChangesAsync();
        
        return new SimpleResponseDto()
        {
            IsGood = true,
            Message = String.Format(Lang.QUIZ_NAME_UPDATED2, prevName, newQuizName),
        };
    }

    public async Task<QuizImagesResDto> UpdateQuizImages(long quizId, List<IFormFile?> uploads, string loggedUsername,
        Controller controller)
    {
        var quizEntity = await _context.Quizes.Include(q => q.UserEntity)
            .FirstOrDefaultAsync(q => q.Id == quizId && q.UserEntity.Username.Equals(loggedUsername));
        if (quizEntity == null) return new QuizImagesResDto()
        {
            IsGood = false,
            Message = Lang.QUIZ_NOT_FOUND
        };
        string dir = $"{FOLDER_PATH}/{quizId}";
        DirectoryInfo directoryInfo = new DirectoryInfo(dir);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        foreach (var file in directoryInfo.GetFiles())
        {
            file.Delete();
        }
        List<QuizImage> quizImages = new List<QuizImage>();
        foreach (var upload in uploads)
        {
            if (upload == null || upload.Length == 0) return new QuizImagesResDto()
            {
                IsGood = true,
                Message = Lang.IMAGE_ERROR
            };
            if (Array.IndexOf(ACCEPTABLE_IMAGE_TYPES, upload.ContentType) == -1) return new QuizImagesResDto()
            {
                IsGood = true,
                Message = String.Format(Lang.IMAGE_ACCEPTED_EXTENSIONS, string.Join(", ", ACCEPTABLE_IMAGE_TYPES))
            };
            string index = Regex.Match(upload.FileName, @"\d+").Value;
            string fullPath = $"{FOLDER_PATH}/{quizId}/question{index}.jpg";
            
            MemoryStream memoryStream = new MemoryStream();
            await upload.CopyToAsync(memoryStream);
            Image image = Image.FromStream(memoryStream);
            image = new Bitmap(image, new Size(500, 500));
            image.Save(fullPath, ImageFormat.Jpeg);
            image.Dispose();
            memoryStream.Close();
            
            string url = $"{Utilities.GetBaseUrl(controller)}/api/v1/dotnet/quizapi/GetQuizImage/{quizId}/{index}";
            quizImages.Add(new QuizImage(){ Id = int.Parse(index), Url = url });
        }
        if (uploads.Count == 0 && Directory.Exists(dir))
        {
            Directory.Delete(dir, true);
        }
        return new QuizImagesResDto()
        {
            IsGood = true,
            QuizImages = quizImages,
            Message = String.Format(Lang.QUIZ_NAME_UPDATED, quizEntity.Name)
        };
    }
}