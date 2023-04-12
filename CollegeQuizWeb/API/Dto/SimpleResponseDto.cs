namespace CollegeQuizWeb.API.Dto;

public class SimpleResponseDto
{
    public bool IsGood { get; set; }
    public string Message { get; set; }
}

public class JoinToSessionDto : SimpleResponseDto
{
    public string QuizName { get; set; }
}