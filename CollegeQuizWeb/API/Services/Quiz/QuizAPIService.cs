using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollegeQuizWeb.API.Dto;
using CollegeQuizWeb.DbConfig;
using CollegeQuizWeb.Entities;
using Microsoft.EntityFrameworkCore;

namespace CollegeQuizWeb.API.Services.Quiz;

public class QuizAPIService : IQuizAPIService
{
    private readonly ApplicationDbContext _context;

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
            Message = "Nie znaleziono quizu przypisanego do Twojego konta."
        };
        // usunięcie poprzednich pytań
        var prevQuestions = _context.Questions.Where(q => q.QuizId.Equals(quizEntity.Id));
        _context.Questions.RemoveRange(prevQuestions);

        // sprawdzenie, czy odpowiedzi nie są takie same
        var flattedQuestions = dto.Aggregate.Select(a => a.Text).ToList();
        if (flattedQuestions.Count != flattedQuestions.Distinct().Count()) return new SimpleResponseDto()
        {
            IsGood = false,
            Message = "Pytania w edytowanym quizie nie mogą się powtarzać."
        };
        foreach (var question in dto.Aggregate)
        {
            int min, sec;
            if (!int.TryParse(question.TimeMin, out min) || !int.TryParse(question.TimeSec, out sec)) 
            {
                return new SimpleResponseDto()
                {
                    IsGood = false,
                    Message = "Podane wartości czasu nie są liczbami."
                };
            }
            if (sec < 5 || sec > 59) return new SimpleResponseDto()
            {
                IsGood = false,
                Message = "Wartość sekund nie może być mniejsza od 10 i większa od 59."
            };
            if (min < 0) min = 0;
            QuestionEntity questionEntity = new QuestionEntity()
            {
                Index = question.Id,
                Name = question.Text,
                TimeMin = min,
                TimeSec = sec,
                QuizEntity = quizEntity
            };
            // sprawdzenie, czy odpowiedzi nie są takie same
            var flattedAnswers = question.Answers.Select(a => a.Text).ToList();
            if (flattedAnswers.Count != flattedAnswers.Distinct().Count()) return new SimpleResponseDto()
            {
                IsGood = false,
                Message = $"Odpowiedzi w pytaniu <strong>{question.Text}</strong> nie mogą być takie same."
            };
            foreach (var answer in question.Answers)
            {
                AnswerEntity answerEntity = new AnswerEntity()
                {
                    Name = answer.Text,
                    IsGood = answer.IsCorrect,
                    QuestionEntity = questionEntity
                };
                await _context.Answers.AddAsync(answerEntity);
            }
            await _context.Questions.AddAsync(questionEntity);
        }
        await _context.SaveChangesAsync();
        return new SimpleResponseDto()
        {
            IsGood = true,
            Message = $"Quiz o nazwie <strong>{quizEntity.Name}</strong> został pomyślnie zaktualizowany."
        };
    }

    public async Task<AggregateQuestionsReqDto> GetQuizQuestions(long quizId, string loggedUsername)
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
                AnswerDto answerDto = new AnswerDto()
                {
                    Id = ++idx,
                    Text = answer.Name,
                    IsCorrect = answer.IsGood
                };
                answerDtos.Add(answerDto);
            }
            QuizQuestionsReqDto questionsReqDto = new QuizQuestionsReqDto()
            {
                Id = question.Index,
                Text = question.Name,
                TimeMin = question.TimeMin.ToString(),
                TimeSec = question.TimeSec.ToString(),
                Type = question.QuestionTypeEntity.Name,
                Answers = answerDtos
            };
            dto.Aggregate.Add(questionsReqDto);
        }
        return dto;
    }

    public async Task<SimpleResponseDto> UpdateQuizName(long quizId, string newQuizName, string loggedUsername)
    {
        var quizEntity = await _context.Quizes.Include(q => q.UserEntity)
            .FirstOrDefaultAsync(q => q.Id == quizId && q.UserEntity.Username.Equals(loggedUsername));
        if (quizEntity == null) return new SimpleResponseDto()
        {
            IsGood = false,
            Message = "Nie znaleziono quizu przypisanego do Twojego konta."
        };
        int countOfSameName = _context.Quizes.Include(q => q.UserEntity)
            .Where(q => q.Name.Equals(newQuizName, StringComparison.OrdinalIgnoreCase)
                        && q.UserEntity.Id.Equals(quizEntity.UserEntity.Id) && q.Id != quizId)
            .Count();
        if (countOfSameName != 0) return new SimpleResponseDto()
        {
            IsGood = false,
            Message = $"Quiz o nazwie <strong>{newQuizName}</strong> istnieje już na Twoim koncie. Podaj inną nazwę."
        };
        string prevName = quizEntity.Name;
        quizEntity.Name = newQuizName;
        _context.Quizes.Update(quizEntity);
        await _context.SaveChangesAsync();
        
        return new SimpleResponseDto()
        {
            IsGood = true,
            Message = $"Nazwa quizu o nazwie <strong>{prevName}</strong> została pomyślnie " +
                      $"zmieniona na <strong>{newQuizName}</strong>",
        };
    }
}