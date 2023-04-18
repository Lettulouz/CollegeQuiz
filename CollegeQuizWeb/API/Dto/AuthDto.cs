namespace CollegeQuizWeb.API.Dto;

public class LoginReqDto
{
    public string UsernameOrEmail { get; set; }
    public string Password { get; set; }
}

public class LoginResDto
{
    public bool IsGood { get; set; }
    public string Token { get; set; }
}