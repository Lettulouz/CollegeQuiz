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