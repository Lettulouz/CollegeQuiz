using System.Collections.Generic;

namespace CollegeQuizWeb.API.Dto;

public class QuizQuestionsReqDto
{
    public int Id { get; set; }
    public string Text { get; set; }
    public string TimeMin { get; set; }
    public string TimeSec { get; set; }
    public List<AnswerDto> Answers { get; set; }
}

public class AggregateQuestionsReqDto
{
    public List<QuizQuestionsReqDto> Aggregate { get; set; }
}