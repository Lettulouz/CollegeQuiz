namespace CollegeQuizWeb.Dto.Quiz;

public class MyQuizDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Author { get; set; }
    public string Token { get; set; }
    public int CountOfQuestions { get; set; }
}