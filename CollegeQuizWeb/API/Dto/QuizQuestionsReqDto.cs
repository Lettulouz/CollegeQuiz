using System.Collections.Generic;
using CollegeQuizWeb.Utils;

namespace CollegeQuizWeb.API.Dto;

public class QuizQuestionsReqDto
{
    public int Id { get; set; }
    public string Text { get; set; }
    public string TimeMin { get; set; }
    public string TimeSec { get; set; }
    public string Type { get; set; }
    public string ImageUrl { get; set; }
    public List<AnswerDto> Answers { get; set; }
}

public class AggregateQuestionReq2Dto
{
    public List<QuizQuestionsReqDto> Aggregate { get; set; }
}

public class AggregateQuestionsReqDto : AggregateQuestionReq2Dto
{
    public List<QuizMode> AvailableModes { get; set; }
    public string PermissionModesMessage { get; set; }
}

public class QuizImage
{
    public int Id { get; set; }
    public string Url { get; set; }
}

public class QuizImagesResDto : SimpleResponseDto
{
    public List<QuizImage> QuizImages { get; set; }
}