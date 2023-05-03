using System.Collections.Generic;

namespace CollegeQuizWeb.Dto.Quiz;

public class LobbyQuestionTick
{
    public long Remaining { get; set; }
    public long Total { get; set; }
}

public class QuizLobbyAnswerData
{
    public string Text { get; set; }
    public bool IsCorrect { get; set; }
}

public class QuizLobbyQuestionData
{
    public string ImageUrl { get; set; }
    public string QuestionName { get; set; }
    public int QuestionId { get; set; }
    public bool IsRange { get; set; }
    public int Max { get; set; }
    public int Min { get; set; }
    public int MaxCounted { get; set; }
    public int MinCounted { get; set; }
    public int QuestionType { get; set; }
    public long TimeSec { get; set; }
    public int Step { get; set; }
    public int CorrectAnswerRange { get; set; }
    public List<QuizLobbyAnswerData> Answers { get; set; } = new List<QuizLobbyAnswerData>();
}