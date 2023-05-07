using System.Collections.Generic;

namespace CollegeQuizWeb.API.Dto;

public class QuizLobbyInfoDto
{
    public string Name { get; set; }
    public string Host { get; set; }
    public int QuestionsCount { get; set; }
}

public class CurrentGameStatusQuestions
{
    public string Username { get; set; }
    public string SelectedAnswer { get; set; }
}

public class ComputePoints
{
    public string Username { get; set; }
    public string Points { get; set; }
    public string IsGood { get; set; }
}

public class BannedDetailsDto
{
    public List<string> BannedUsers { get; set; }
    public List<string> UnbannedUsers { get; set; }
}

public class QuizManagerDto
{
    public int QuestionId { get; set; }
    public string Question { get; set; }
    public int QuestionType { get; set; }
    public List<string> Answers { get; set; }
    public long TimeSec { get; set; }
    public bool IsRange { get; set; }
    public int Step { get; set; }
    public int Min { get; set; }
    public int MinCounted { get; set; }
    public int Max { get; set; }
    public int MaxCounted { get; set; }
    public string ImageUrl { get; set; }
}