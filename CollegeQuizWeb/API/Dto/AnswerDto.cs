namespace CollegeQuizWeb.API.Dto;

public class AnswerDto
{
    public long Id { get; set; }
    public string Text { get; set; }
    public bool IsCorrect { get; set; }
    
    public bool IsRange { get; set; }
    
    public int Step { get; set; }
    
    public int MinCounted { get; set; }
    
    public int MaxCounted { get; set; }
    public int Min { get; set; }
    
    public int Max { get; set; }
    public int CorrectAns { get; set; }
}